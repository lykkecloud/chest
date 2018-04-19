// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a service to store and retrieve keyvalue pairs in the data store
    /// </summary>
    public class MetadataService : IMetadataService
    {
        private Dictionary<string, KeyValueData> dataStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataService"/> class.
        /// </summary>
        public MetadataService()
        {
            this.dataStore = new Dictionary<string, KeyValueData>();
        }

        /// <summary>
        /// Get key value dictionary data for a given key
        /// </summary>
        /// <param name="key">The key for which to get the dictionary data</param>
        /// <returns>A typed <see cref="Dictionary{TKey, TValue}"/> object</returns>
        public Task<Dictionary<string, string>> GetAsync(string key)
        {
            if (this.dataStore.TryGetValue(key, out var keyValue))
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValue.SerializedData);
                return Task.FromResult(data);
            }

            return Task.FromResult(default(Dictionary<string, string>));
        }

        /// <summary>
        /// Stores key value pair data against a given key
        /// </summary>
        /// <param name="key">The key for which to store key value pair data</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> object representing key value pair data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        public Task<bool> SaveAsync(string key, Dictionary<string, string> data)
        {
            var serializedData = JsonConvert.SerializeObject(data);

            var isAdded = this.dataStore.TryAdd(key, new KeyValueData { Key = key, SerializedData = serializedData });

            return Task.FromResult(isAdded);
        }
    }
}
