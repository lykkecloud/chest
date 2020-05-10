// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chest.Data.Entities
{
    /// <summary>
    /// Represents a key value pair to be stored in data store
    /// </summary>
    [Table("tb_keyValueData", Schema = "chest")]
    internal class KeyValueData
    {
        /// <summary>
        /// Gets or sets category
        /// </summary>
        [MaxLength(100)]
        [Column(Order = 0)]
        [DefaultValue("metadata")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets collection
        /// </summary>
        [MaxLength(100)]
        [Column(Order = 1)]
        [DefaultValue("metadata")]
        public string Collection { get; set; }

        /// <summary>
        /// Gets or sets Key
        /// </summary>
        [MaxLength(100)]
        [Column(Order = 2)]
        public string Key { get; set; }

        [Required]
        [MaxLength(100)]
        [DefaultValue("metadata")]
        public string DisplayCategory { get; set; }

        [Required]
        [MaxLength(100)]
        [DefaultValue("metadata")]
        public string DisplayCollection { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayKey { get; set; }

        /// <summary>
        /// Gets or sets the json data associated with the given key
        /// </summary>
        [Required]
        [MaxLength(4096)]
        public string MetaData { get; set; }

        /// <summary>
        /// Gets or sets json keywords associated with the meta data
        /// </summary>
        [MaxLength(1024)]
        public string Keywords { get; set; }

        public override string ToString() => $"{this.Key}:{this.MetaData}";
        
        public static Func<KeyValueData, bool> SelectAllKeysInCollection(string category, string collection) =>
            k =>
            {
                var keys = new KeyValueDataKeys(category, collection);
                return k.Category == keys.Category && k.Collection == keys.Collection;
            };

        public static IEnumerable<string> GetNormalizedDbKeys(string category, string collection, string key)
        {
            var primaryKey = new KeyValueDataKeys(category, collection, key);

            yield return primaryKey.Category;
            yield return primaryKey.Collection;
            yield return primaryKey.Key;
        }

        public static KeyValueData Create(string category, 
            string collection, 
            string key, 
            string metadata, 
            string keywords)
        {
            var primaryKey = new KeyValueDataKeys(category, collection, key);
            
            return new KeyValueData
            {
                Category = primaryKey.Category,
                Collection = primaryKey.Collection,
                Key = primaryKey.Key,
                DisplayCategory = category,
                DisplayCollection = collection,
                DisplayKey = key,
                MetaData = metadata,
                Keywords = keywords
            };
        }
    }
}
