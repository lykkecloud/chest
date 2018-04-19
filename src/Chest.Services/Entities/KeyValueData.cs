// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Services
{
    /// <summary>
    /// Represents a key value pair to be stored in data store
    /// </summary>
    internal class KeyValueData
    {
        /// <summary>
        /// Gets or sets Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the json data associated with the given key
        /// </summary>
        public string SerializedData { get; set; }
    }
}
