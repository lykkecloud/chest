// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Chest.Data.Entities;

namespace Chest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Exceptions;
    using EFSecondLevelCache.Core;
    using EFSecondLevelCache.Core.Contracts;
    using Microsoft.EntityFrameworkCore;
    using Serilog;

    /// <summary>
    /// Represents a service to store and retrieve key-value pairs in the data store
    /// </summary>
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEFCacheServiceProvider _cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="ApplicationDbContext"/> to communicate with underlying database</param>
        /// <param name="cacheProvider">An instance of <see cref="IEFCacheServiceProvider"/> to control EfCore 2nd level cache</param>
        public DataService(ApplicationDbContext context, IEFCacheServiceProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Gets data for a given key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key for which to get the data</param>
        /// <returns>A tuple of <see cref="string"/> with the key data and <see cref="string"/> with the keywords associated with the key</returns>
        public async Task<(string data, string keywords)> Get(string category, string collection, string key)
        {
            var dbKeys = KeyValueData.GetNormalizedDbKeys(category, collection, key);
            var data = await _context.KeyValues.FindAsync(dbKeys.ToArray<object>());
            return (data?.MetaData, data?.Keywords);
        }

        /// <summary>
        /// Stores key value pair data against a given category, collection and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key value for which to store key data</param>
        /// <param name="data">A <see cref="string"/> representing the key data</param>
        /// <param name="keywords">A <see cref="string"/> representing the Keywords associated with the data, these keywords will be used to search the data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        /// <exception cref="DuplicateKeyException">if a record already exist against category, collection and key</exception>
        public async Task Add(string category, string collection, string key, string data, string keywords)
        {
            if (!await KeyExistsIncludingWarning(category, collection, key))
            {
                await _context.AddAsync(KeyValueData.Create(category, collection, key, data, keywords));
                await _context.SaveChangesAsync();
                _cacheProvider.ClearAllCachedEntries();
            }
        }

        /// <inheritdoc />
        public async Task BulkAdd(string category, string collection, Dictionary<string, (string metadata, string keywords)> data)
        {
            var existingKeysResult = await AnyKeyExistsIncludingWarning(category, collection, data.Keys.ToList());

            if (existingKeysResult.anyExists)
            {
                var keysToExclude = existingKeysResult.existingKeys.ToHashSet();
                data = data.Where(x => !keysToExclude.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            }

            if (data.Any())
            {
                foreach (var kvp in data)
                {
                    await _context.AddAsync(KeyValueData.Create(
                        category, collection, kvp.Key, kvp.Value.metadata, kvp.Value.keywords));
                }

                await _context.SaveChangesAsync();
                _cacheProvider.ClearAllCachedEntries();
            }
        }

        /// <summary>
        /// Updates key value pair data against a given category, collection and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key value for which to store key data</param>
        /// <param name="data">A <see cref="string"/> representing the key data</param>
        /// <param name="keywords">A <see cref="string"/> representing the Keywords associated with the data, these keywords will be used to search the data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        /// <exception cref="NotFoundException">if no record found to update</exception>
        public async Task Upsert(string category, string collection, string key, string data, string keywords)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException(nameof(category));
            
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentNullException(nameof(collection));
            
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var dbKeys = KeyValueData.GetNormalizedDbKeys(category, collection, key);
                    var existingKeyEntity = await _context.KeyValues.FindAsync(dbKeys.ToArray<object>());
                    
                    if (existingKeyEntity == null)
                    {
                        await _context.AddAsync(KeyValueData.Create(category, collection, key, data, keywords));
                    }
                    else
                    {
                        existingKeyEntity.MetaData = data;
                        existingKeyEntity.Keywords = keywords;
                    }

                    await _context.SaveChangesAsync();
                    
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Couldn't make upsert for category [{category}]," + 
                                 $" collection [{collection}] and key [{key}]");

                    throw;
                }
            }
            
            _cacheProvider.ClearAllCachedEntries();
        }
        
        /// <inheritdoc />
        public async Task BulkUpsert(string category, string collection, Dictionary<string, (string metadata, string keywords)> updatedData)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException(nameof(category));
            
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentNullException(nameof(collection));
            
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var collectionExists = await HasKeyValueCollection(category, collection);
                    
                    if (collectionExists)
                    {
                        var existingData = await GetKeyValueDataByKeys(category, collection, updatedData.Keys)
                            .ToDictionaryAsync(x => x.Key, x => x);

                        var missingKeys = updatedData.Keys
                            .Select(KeyValueDataKeys.NormalizeValue)
                            .Except(existingData.Keys)
                            .ToList();

                        if (missingKeys.Any())
                        {
                            throw new NotFoundException(
                                $"The following keys were not found in category '{category}' and collection '{collection}': {string.Join(", ", missingKeys)}");
                        }
                    
                        foreach (var updatedDataRow in updatedData)
                        {
                            var normalizedKey = KeyValueDataKeys.NormalizeValue(updatedDataRow.Key);
                            var keyValueToUpdate = existingData[normalizedKey];
                            keyValueToUpdate.MetaData = updatedDataRow.Value.metadata;
                            keyValueToUpdate.Keywords = updatedDataRow.Value.keywords;

                            _context.Update(keyValueToUpdate);
                        }
                    }
                    else
                    {
                        foreach (var updatedDataRow in updatedData)
                        {
                            var item = KeyValueData.Create(category, collection, updatedDataRow.Key,
                                updatedDataRow.Value.metadata, updatedDataRow.Value.keywords);
                            
                            await _context.AddAsync(item);
                        }
                    }

                    await _context.SaveChangesAsync();
                    
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Couldn't make bulk update for category [{category}]" + 
                                 $" and collection [{collection}], number of keys to update [{updatedData?.Count ?? 0}]");

                    throw;
                }
                
                _cacheProvider.ClearAllCachedEntries();
            }
        }

        /// <inheritdoc />
        public async Task BulkReplace(string category, string collection, Dictionary<string, (string metadata, string keywords)> updatedData)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentNullException(nameof(category));
            
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentNullException(nameof(collection));
            
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var allKeysInCollection = _context.KeyValues
                        .Where(KeyValueData.SelectAllKeysInCollection(category, collection))
                        .ToList();
                    
                    if (allKeysInCollection.Count > 0)
                        _context.KeyValues.RemoveRange(allKeysInCollection);

                    if (updatedData?.Any() ?? false)
                    {
                        var newKeys = updatedData.Select(x =>
                            KeyValueData.Create(category, collection, x.Key, x.Value.metadata, x.Value.keywords));
                        
                        await _context.KeyValues.AddRangeAsync(newKeys);
                    }

                    await _context.SaveChangesAsync();
                    
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Couldn't make bulk replace for category [{category}]" + 
                                 $" and collection [{collection}], number of keys to update [{updatedData?.Count ?? 0}]");

                    throw;
                }
                
                _cacheProvider.ClearAllCachedEntries();
            }
        }

        /// <summary>
        /// Deletes a record by category, collection, and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key name</param>
        /// <returns>A <see cref="Task"/> representing the operation</returns>
        public async Task Delete(string category, string collection, string key)
        {
            var dbKeys = KeyValueData.GetNormalizedDbKeys(category, collection, key);
            var existing = await _context.KeyValues.FindAsync(dbKeys.ToArray<object>());
            if (existing == null)
            {
                return;
            }

            _context.KeyValues.Remove(existing);

            await _context.SaveChangesAsync();
            _cacheProvider.ClearAllCachedEntries();
        }

        /// <inheritdoc />
        public async Task BulkDelete(string category, string collection, IEnumerable<string> keys)
        {
            foreach (var elem in await GetKeyValueDataByKeys(category, collection, keys).ToArrayAsync())
            {
                _context.KeyValues.Remove(elem);
            }

            await _context.SaveChangesAsync();
            _cacheProvider.ClearAllCachedEntries();
        }

        /// <summary>
        /// Gets all distinct categories in the system
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of unique categories in the system</returns>
        public Task<List<string>> GetCategories()
        {
            return _context
                .KeyValues
                .Select(k => k.DisplayCategory)
                .Distinct()
                .Cacheable()
                .ToListAsync();
        }

        /// <summary>
        /// Gets distinct collections against the given category
        /// </summary>
        /// <param name="category">The category for which to get the collections</param>
        /// <returns>A <see cref="List{T}"/> of unique collections</returns>
        public Task<List<string>> GetCollections(string category)
        {
            var normalizedKey = new KeyValueDataKeys(category);
            
            return _context
                .KeyValues
                .Where(k => k.Category == normalizedKey.Category)
                .Select(k => k.DisplayCollection)
                .Distinct()
                .Cacheable()    
                .ToListAsync();
        }

        /// <summary>
        /// Gets key values in the system against the category and collection
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="keyword">Optional param to search key value pairs</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of key and data</returns>
        public Task<Dictionary<string, string>> GetKeyValues(string category, string collection, string keyword = null)
        {
            var normalizedData = new KeyValueDataKeys(category, collection);

            return _context
                .KeyValues
                .Where(k => k.Category == normalizedData.Category && k.Collection == normalizedData.Collection)
                .Cacheable()
                .Where(k => string.IsNullOrWhiteSpace(keyword) || k.Keywords != null &&
                    k.Keywords.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                .ToDictionaryAsync(
                    k => k.DisplayKey,
                    k => k.MetaData);
        }

        /// <inheritdoc />
        public Task<Dictionary<string, string>> FindByKeys(string category, string collection, IEnumerable<string> keys, string keyword = null)
        {
            return GetKeyValueDataByKeys(category, collection, keys, keyword)
                .ToDictionaryAsync(
                    k => k.DisplayKey,
                    k => k.MetaData);
        }

        private IQueryable<KeyValueData> GetKeyValueDataByKeys(string category, string collection,
            IEnumerable<string> keys, string keyword = null)
        {
            var normalizedData = new KeyValueDataKeys(category, collection);

            var normalizedKeys = keys.Select(KeyValueDataKeys.NormalizeValue);

            return _context
                .KeyValues
                .Where(k => k.Category == normalizedData.Category && k.Collection == normalizedData.Collection)
                .Cacheable()
                .Where(k => normalizedKeys.Contains(k.Key))
                .Where(k => string.IsNullOrWhiteSpace(keyword) || k.Keywords != null &&
                    k.Keywords.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
        }

        private Task<bool> HasKeyValueCollection(string category, string collection)
        {
            var normalizedData = new KeyValueDataKeys(category, collection);

            return _context
                .KeyValues
                .AnyAsync(k => k.Category == normalizedData.Category && k.Collection == normalizedData.Collection);
        }

        private async Task<bool> KeyExistsIncludingWarning(string category, string collection, string key)
        {
            return (await AnyKeyExistsIncludingWarning(category, collection, new List<string> { key })).anyExists;
        }

        private async Task<(bool anyExists, IEnumerable<string> existingKeys)> AnyKeyExistsIncludingWarning(
            string category,
            string collection,
            IEnumerable<string> keys)
        {
            var existingKeys = await GetKeyValueDataByKeys(category, collection, keys)
                .Select(x => x.Key)
                .ToListAsync();

            if (existingKeys.Any())
            {
                Log.Warning($"Key(s) {string.Join(",", existingKeys)} already exist(s) in Category: '{category}' Collection: '{collection}'");
            }

            return (existingKeys.Any(), existingKeys);
        }
    }
}
