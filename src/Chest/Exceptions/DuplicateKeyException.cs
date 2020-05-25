// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Exceptions
{
    using System;

    /// <summary>
    /// Represents duplicate key exception
    /// </summary>
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException()
            : this(null, null)
        {
        }

        public DuplicateKeyException(string message)
            : this(message, null)
        {
        }

        public DuplicateKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateKeyException(string category, string collection, string key, string message, Exception innerException)
            : base(message, innerException)
        {
            Category = category;
            Collection = collection;
            Key = key;
        }

        public string Category { get; set; }

        public string Collection { get; set; }

        public string Key { get; set; }
    }
}
