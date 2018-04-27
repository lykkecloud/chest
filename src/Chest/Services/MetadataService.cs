// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    using System.Collections.Generic;
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
        /// <param name="key">The key for which to get the dictionary data</param>
        /// <returns>A typed <see cref="Dictionary{TKey, TValue}"/> object</returns>
        public async Task<Dictionary<string, string>> Get(string key)
        {
            var data = await this.context.KeyValues.FindAsync(key.ToUpperInvariant());

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
                    throw new DuplicateKeyException(key, $"Cannot add Key: {key} because it already exists.", dbException);
                }

                throw;
            }
        }
    }
}
