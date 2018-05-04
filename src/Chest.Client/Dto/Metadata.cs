// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Dto
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the data model
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets key
        /// </summary>
        public string Key { get; set; }

#pragma warning disable CA2227

        /// <summary>
        /// Gets or sets data against the given key
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}
