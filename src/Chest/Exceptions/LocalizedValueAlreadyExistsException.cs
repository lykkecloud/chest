using System;
using Chest.Data.Entities;

namespace Chest.Exceptions
{
    public class LocalizedValueAlreadyExistsException : Exception
    {
        public LocalizedValueAlreadyExistsException(string locale, string key) 
            : base($"Localized value {locale}, {key} already exists")
        {
        }

        public LocalizedValueAlreadyExistsException(LocalizedValue value) : this(value.Locale, value.Key)
        {
        }
    }
}