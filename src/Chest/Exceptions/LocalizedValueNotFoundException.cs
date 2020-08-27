using System;
using Chest.Data.Entities;

namespace Chest.Exceptions
{
    public class LocalizedValueNotFoundException : Exception
    {
        public LocalizedValueNotFoundException(string locale, string key) 
            : base($"Localized value {locale}, {key} not found")
        {
        }

        public LocalizedValueNotFoundException(LocalizedValue value) : this(value.Locale, value.Key)
        {
        }
    }
}