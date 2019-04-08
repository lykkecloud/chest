// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Exceptions
{
    using System;

    /// <summary>
    /// Represents key not found exception
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : this(null, null)
        {
        }

        public NotFoundException(string message)
            : this(message, null)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string category, string collection, string key, string message, Exception innerException)
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
