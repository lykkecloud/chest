// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a key value pair to be stored in data store
    /// </summary>
    [Table("key_value_data")]
    internal class KeyValueData
    {
        /// <summary>
        /// Gets or sets Key
        /// </summary>
        [Key]
        [MaxLength(100)]
        [Column("key")]
        public string Key { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("display_key")]
        public string DisplayKey { get; set; }

        /// <summary>
        /// Gets or sets the json data associated with the given key
        /// </summary>
        [Required]
        [MaxLength(4096)]
        [Column("metadata_data")]
        public string MetaData { get; set; }

        public override string ToString() => $"{this.Key}:{this.MetaData}";
    }
}
