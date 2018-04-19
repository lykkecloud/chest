// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service to store and retrieve keyvalue pairs in the data store
    /// </summary>
    public interface IMetadataService
    {
        /// <summary>
        /// Get key value dictionary data for a given key
        /// </summary>
        /// <param name="key">The key for which to get the dictionary data</param>
        /// <returns>A typed <see cref="Dictionary{TKey, TValue}"/> object</returns>
        Task<Dictionary<string, string>> GetAsync(string key);

        /// <summary>
        /// Stores key value pair data against a given key
        /// </summary>
        /// <param name="key">The key for which to store key value pair data</param>
        /// <param name="data">The key value pair data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        Task<bool> SaveAsync(string key, Dictionary<string, string> data);
    }
}
