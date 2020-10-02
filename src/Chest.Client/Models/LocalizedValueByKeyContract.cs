using System.Collections.Generic;

namespace Chest.Client.Models
{
    public class LocalizedValueByKeyContract
    {
        public string Key { get; set; }
        public IReadOnlyDictionary<string, string> Localizations { get; set; }
    }
}