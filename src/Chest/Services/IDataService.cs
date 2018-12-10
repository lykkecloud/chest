// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1716

namespace Chest.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service to store and retrieve key and values in the data store
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets data for a given key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key for which to get the data</param>
        /// <returns>A tuple of <see cref="string"/> with the key data and <see cref="string"/> with the keywords associated with the key</returns>
        Task<(string data, string keywords)> Get(string category, string collection, string key);

        /// <summary>
        /// Stores key value pair data against a given key
        /// </summary>
        /// <param name="key">The key value for which to store key data</param>
        /// <param name="data">A <see cref="string"/> representing key data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        Task Save(string key, string data);

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
        Task Add(string category, string collection, string key, string data, string keywords);

        /// <summary>
        /// Stores a set of key value pair datas against a given category and collection
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="data">The set of keys and associated metadata and keywords to store</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        /// <exception cref="Exceptions.DuplicateKeyException"> if one or more of the keys already exist in the collection</exception>
        Task BulkAdd(string category, string collection, Dictionary<string, (string metadata, string keywords)> data);

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
        Task Update(string category, string collection, string key, string data, string keywords);

        /// <summary>
        /// Updates multiple sets of key value pairs in a given category and collection
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="data">A <see cref="Dictionary{TKey, TValue}"/> containing the keys to update the metadata and keywords for</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation </returns>
        /// <exception cref="Chest.Exceptions.NotFoundException">Thrown when some of the keys weren't found</exception>
        Task BulkUpdate(string category, string collection, Dictionary<string, (string metadata, string keywords)> data);

        /// <summary>
        /// Deletes a record by category, collection, and key
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="key">The key name</param>
        /// <returns>A <see cref="Task"/> representing the operation</returns>
        Task Delete(string category, string collection, string key);

        /// <summary>
        /// Deletes a set of records by category, collection, and keys
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="keys">The keys</param>
        /// <returns>A <see cref="Task"/> representing the operation</returns>
        Task BulkDelete(string category, string collection, IEnumerable<string> keys);

        /// <summary>
        /// Gets all distinct categories in the system
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of unique categories in the system</returns>
        Task<List<string>> GetCategories();

        /// <summary>
        /// Gets distinct collections against the given category
        /// </summary>
        /// <param name="category">The category for which to get the collections</param>
        /// <returns>A <see cref="List{T}"/> of unique collections</returns>
        Task<List<string>> GetCollections(string category);

        /// <summary>
        /// Gets key values in the system against the category and collection
        /// </summary>
        /// <param name="category">The category</param>
        /// <param name="collection">The collection</param>
        /// <param name="keyword">Optional param to search key value pairs</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of key and data</returns>
        Task<Dictionary<string, string>> GetKeyValues(string category, string collection, string keyword = null);

        /// <summary>
        /// Looks up a set of metadata key value pairs in a specified category and collection using a set of keys to search for
        /// </summary>
        /// <param name="category">The category to search in</param>
        /// <param name="collection">The collection to search in</param>
        /// <param name="keys">The set of keys to search for</param>
        /// <param name="keyword">An optional keyword to narrow down the search</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of key and metadata</returns>
        Task<Dictionary<string, string>> FindByKeys(string category, string collection, IEnumerable<string> keys, string keyword = null);
    }
}
