using System.Collections.Generic;
using System.Linq;
using Chest.Data.Entities;

namespace Chest.Models.v2.LocalizedValues
{
    public class LocalizedValueByKey
    {
        public string Key { get; }
        public Dictionary<string, string> Localizations { get; }

        public LocalizedValueByKey(string key, List<LocalizedValue> localizedValues)
        {
            Key = key;
            Localizations = localizedValues.ToDictionary(g => g.Locale, g => g.Value);
        }
    }
}