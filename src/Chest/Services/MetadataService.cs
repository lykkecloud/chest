// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chest.Data;
    using Chest.Exceptions;
    using Chest.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Npgsql;

    /// <summary>
    /// Represents a service to store and retrieve keyvalue pairs in the data store
    /// </summary>
    public class MetadataService : IMetadataService
    {
        private const string PostgresDuplicateKeyErrorCode = "23505";

        private readonly ApplicationDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataService"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="ApplicationDbContext"/> to communicate with underlying database</param>
        public MetadataService(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets key value dictionary data for a given key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key for which to get the dictionary data</param>
        /// <returns>A typed <see cref="Dictionary{TKey, TValue}"/> object</returns>
        public async Task<Dictionary<string, string>> Get(string category, string collection, string key)
        {
            var data = await this.context.KeyValues.FindAsync(category.ToUpperInvariant(), collection.ToUpperInvariant(), key.ToUpperInvariant());

            if (!string.IsNullOrEmpty(data?.MetaData))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(data.MetaData);
            }

            return default(Dictionary<string, string>);
        }

        /// <summary>
        /// Stores key value pair data against a given key
        /// </summary>
        /// <param name="key">The key for which to store key value pair data</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> object representing key value pair data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        public async Task Save(string key, Dictionary<string, string> data)
        {
            var serializedData = JsonConvert.SerializeObject(data);

            try
            {
                var isAdded = await this.context.AddAsync(new KeyValueData
                {
                    Key = key.ToUpperInvariant(),
                    DisplayKey = key,
                    MetaData = serializedData
                });

                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateException dbException)
            {
                if (dbException.InnerException is PostgresException e && e.SqlState == PostgresDuplicateKeyErrorCode)
                {
                    throw new DuplicateKeyException("", "", key, $"Cannot add Key: {key} because it already exists.", dbException);
                }

                throw;
            }
        }

        /// <summary>
        /// Stores key value pair data against a given category, collection and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key for which to store key value pair data</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> object representing key value pair data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        /// <exception cref="DuplicateKeyException">if a record already exist against category, collection and key</exception>
        public async Task Add(string category, string collection, string key, Dictionary<string, string> data)
        {
            var serializedData = JsonConvert.SerializeObject(data);

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
                    MetaData = serializedData
                });

                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateException dbException)
            {
                if (dbException.InnerException is PostgresException e && e.SqlState == PostgresDuplicateKeyErrorCode)
                {
                    throw new DuplicateKeyException(category, collection, key, $"Cannot insert duplicate for Category: {category} Collection: {collection} Key: {key}.", dbException);
                }

                throw;
            }
        }

        /// <summary>
        /// Updates key value pair data against a given category, collection and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key for which to store key value pair data</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> object representing key value pair data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        /// <exception cref="NotFoundException">if no record found to update</exception>
        public async Task Update(string category, string collection, string key, Dictionary<string, string> data)
        {
            var serializedData = JsonConvert.SerializeObject(data);

            var existing = await this.context.KeyValues.FindAsync(category.ToUpperInvariant(), collection.ToUpperInvariant(), key.ToUpperInvariant());

            if (existing == null)
            {
                throw new NotFoundException(category, collection, key, $"No record found for Category: {category} Collection: {collection} and Key: {key}", null);
            }

            existing.MetaData = serializedData;

            await this.context.SaveChangesAsync();
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
                .ToListAsync();
        }

        /// <summary>
        /// Gets key values in the system against the category and collection
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of key and metadata</returns>
        public Task<Dictionary<string, Dictionary<string, string>>> GetKeyValues(string category, string collection)
        {
            return this.context
                .KeyValues
                .Where(k => k.Category == category.ToUpperInvariant() && k.Collection == collection.ToUpperInvariant())
                .ToDictionaryAsync(
                    k => k.DisplayKey,
                    k => JsonConvert.DeserializeObject<Dictionary<string, string>>(k.MetaData));
        }
    }
}
