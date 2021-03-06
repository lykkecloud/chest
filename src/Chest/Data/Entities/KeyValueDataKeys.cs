using System;

namespace Chest.Data.Entities
{
    internal readonly struct KeyValueDataKeys
    {
        public string Category { get; }
        public string Collection { get; }
        public string Key { get; }

        public KeyValueDataKeys(string category = null, string collection = null, string key = null)
        {
            if (string.IsNullOrEmpty(category) &&
                string.IsNullOrEmpty(collection) &&
                string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("At least one key must be not null or empty");
            }
            
            Category = NormalizeValue(category);
            Collection = NormalizeValue(collection);
            Key = NormalizeValue(key);
        }

        public static string NormalizeValue(string value) => value?.ToUpperInvariant();
    }
}