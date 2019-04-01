// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

#pragma warning disable CA2227
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Models.v2
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the data model
    /// </summary>
    public class MetadataModel
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
