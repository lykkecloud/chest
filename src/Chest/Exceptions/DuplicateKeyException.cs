// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

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
            this.Category = category;
            this.Collection = collection;
            this.Key = key;
        }

        public string Category { get; set; }

        public string Collection { get; set; }

        public string Key { get; set; }
    }
}
