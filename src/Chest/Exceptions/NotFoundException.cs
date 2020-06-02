// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

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
            Category = category;
            Collection = collection;
            Key = key;
        }

        public string Category { get; set; }

        public string Collection { get; set; }

        public string Key { get; set; }
    }
}
