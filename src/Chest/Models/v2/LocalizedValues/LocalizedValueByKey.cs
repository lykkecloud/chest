using System.Collections.Generic;
using System.Linq;
using Chest.Data.Entities;

namespace Chest.Models.v2.LocalizedValues
{
    public class LocalizedValueByKey
    {
        public string Key { get; set; }
        public Dictionary<string, string> Localizations { get; set; }

        public LocalizedValueByKey(string key, List<LocalizedValue> localizedValues)
        {
            Key = key;
            Localizations = localizedValues.ToDictionary(g => g.Locale, g => g.Value);
        }
    }
}