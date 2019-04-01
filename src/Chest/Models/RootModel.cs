// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the data model
    /// </summary>
    public class RootModel
    {
        /// <summary>
        /// Gets or sets title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets version
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets OS
        /// </summary>
        [Required]
        public string OS { get; set; }

        /// <summary>
        /// Gets or sets process id
        /// </summary>
        [Required]
        public int ProcessId { get; set; }
    }
}
