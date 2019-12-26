// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA2227
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Client.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the data model
    /// </summary>
    public class MetadataModelContract
    {
        /// <summary>
        /// Gets or sets data against the given key
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Provide a value for key data")]
        public string Data { get; set; }

        public string Keywords { get; set; }
    }
}
