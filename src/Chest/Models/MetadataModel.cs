// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the data model
    /// </summary>
    public class MetadataModel
    {
#pragma warning disable CA2227

        /// <summary>
        /// Gets or sets data against the given key
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Provide at least one key-value pair in data")]
        public Dictionary<string, string> Data { get; set; }
    }
}
