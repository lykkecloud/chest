// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Chest.Data;
    using Chest.Exceptions;
    using EFSecondLevelCache.Core;
    using EFSecondLevelCache.Core.Contracts;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a service to store and retrieve key-value pairs in the data store
    /// </summary>
    public class DataService : IDataService
    {
        private readonly int[] sqlDuplicateKeyErrorCodes = new[] { 2601, 2627, 547 };

        private readonly ApplicationDbContext context;

        private readonly IEFCacheServiceProvider cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="ApplicationDbContext"/> to communicate with underlying database</param>
        /// <param name="cacheProvider">An instance of <see cref="IEFCacheServiceProvider"/> to control EfCore 2nd level cache</param>
        public DataService(ApplicationDbContext context, IEFCacheServiceProvider cacheProvider)
        {
            this.context = context;
            this.cacheProvider = cacheProvider;
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
            var data = await this.context.KeyValues.FindAsync(category.ToUpperInvariant(), collection.ToUpperInvariant(), key.ToUpperInvariant());
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
            try
            {
                var isAdded = await this.context.AddAsync(new KeyValueData
                {
                    Category = category.ToUpperInvariant(),
                    Collection = collection.ToUpperInvariant(),
                    Key = key.ToUpperInvariant(),
                    DisplayCategory = category,
                    DisplayCollection = collection,
                    DisplayKey = key,
                    MetaData = data,
                    Keywords = keywords,
                });

                await this.context.SaveChangesAsync();
                this.cacheProvider.ClearAllCachedEntries();
            }
            catch (DbUpdateException dbException)
            {
                if (dbException.InnerException is SqlException e && this.sqlDuplicateKeyErrorCodes.Contains(e.Number))
                {
                    throw new DuplicateKeyException(category, collection, key, $"Cannot insert duplicate for Category: {category} Collection: {collection} Key: {key}.", dbException);
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task BulkAdd(string category, string collection, Dictionary<string, (string metadata, string keywords)> data)
        {
            try
            {
                foreach (var kvp in data)
                {
                    await this.context.AddAsync(new KeyValueData
                    {
                        Category = category.ToUpperInvariant(),
                        Collection = collection.ToUpperInvariant(),
                        Key = kvp.Key.ToUpperInvariant(),
                        DisplayCategory = category,
                        DisplayCollection = collection,
                        DisplayKey = kvp.Key,
                        MetaData = kvp.Value.metadata,
                        Keywords = kvp.Value.keywords,
                    });
                }

                await this.context.SaveChangesAsync();
                this.cacheProvider.ClearAllCachedEntries();
            }
            catch (DbUpdateException dbException)
            {
                if (dbException.InnerException is SqlException e && this.sqlDuplicateKeyErrorCodes.Contains(e.Number))
                {
                    throw new DuplicateKeyException($"Some of the keys already exist in Category: '{category}' Collection: '{collection}'", dbException);
                }

                throw;
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
        public async Task Update(string category, string collection, string key, string data, string keywords)
        {
            var existing = await this.context.KeyValues.FindAsync(category.ToUpperInvariant(), collection.ToUpperInvariant(), key.ToUpperInvariant());
            if (existing == null)
            {
                throw new NotFoundException(category, collection, key, $"No record found for Category: {category} Collection: {collection} and Key: {key}", null);
            }

            existing.MetaData = data;
            existing.Keywords = keywords;

            await this.context.SaveChangesAsync();
            this.cacheProvider.ClearAllCachedEntries();
        }

        /// <inheritdoc />
        public async Task BulkUpdate(string category, string collection, Dictionary<string, (string metadata, string keywords)> data)
        {
            var keyValueData = await this.GetKeyValueDataByKeys(category, collection, data.Keys).ToDictionaryAsync(
                x => x.Key,
                x => x);

            var missingKeys = data.Keys.Select(k => k.ToUpperInvariant()).Except(keyValueData.Keys);

            if (missingKeys.Any())
            {
                throw new NotFoundException($"The following keys were not found in category '{category}' and collection '{collection}': {string.Join(", ", missingKeys)}");
            }

            foreach (var kvp in data)
            {
                var keyValue = keyValueData[kvp.Key.ToUpperInvariant()];

                keyValue.MetaData = kvp.Value.metadata;
                keyValue.Keywords = kvp.Value.keywords;
            }

            await this.context.SaveChangesAsync();
            this.cacheProvider.ClearAllCachedEntries();
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
            var existing = await this.context.KeyValues.FindAsync(category.ToUpperInvariant(), collection.ToUpperInvariant(), key.ToUpperInvariant());
            if (existing == null)
            {
                return;
            }

            this.context.KeyValues.Remove(existing);

            await this.context.SaveChangesAsync();
            this.cacheProvider.ClearAllCachedEntries();
        }

        /// <inheritdoc />
        public async Task BulkDelete(string category, string collection, IEnumerable<string> keys)
        {
            foreach (var elem in await this.GetKeyValueDataByKeys(category, collection, keys).ToArrayAsync())
            {
                this.context.KeyValues.Remove(elem);
            }

            await this.context.SaveChangesAsync();
            this.cacheProvider.ClearAllCachedEntries();
        }

        /// <summary>
        /// Gets all distinct categories in the system
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of unique categories in the system</returns>
        public Task<List<string>> GetCategories()
        {
            return this.context
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
            return this.context
                .KeyValues
                .Where(k => k.Category == category.ToUpperInvariant())
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
            return this.context
                .KeyValues
                .Where(k => k.Category == category.ToUpperInvariant() && k.Collection == collection.ToUpperInvariant())
                .Cacheable()
                .Where(k => string.IsNullOrWhiteSpace(keyword) || (k.Keywords != null && k.Keywords.ToUpperInvariant().Contains(keyword.ToUpperInvariant())))
                .ToDictionaryAsync(
                    k => k.DisplayKey,
                    k => k.MetaData);
        }

        /// <inheritdoc />
        public Task<Dictionary<string, string>> FindByKeys(string category, string collection, IEnumerable<string> keys, string keyword = null)
        {
            return this.GetKeyValueDataByKeys(category, collection, keys, keyword)
                .ToDictionaryAsync(
                    k => k.DisplayKey,
                    k => k.MetaData);
        }

        private IQueryable<KeyValueData> GetKeyValueDataByKeys(string category, string collection, IEnumerable<string> keys, string keyword = null)
        {
            return this.context
                .KeyValues
                .Where(k => k.Category == category.ToUpperInvariant() && k.Collection == collection.ToUpperInvariant())
                .Cacheable()
                .Where(k => keys.Contains(k.Key))
                .Where(k => string.IsNullOrWhiteSpace(keyword) || (k.Keywords != null && k.Keywords.ToUpperInvariant().Contains(keyword.ToUpperInvariant())));
        }
    }
}
