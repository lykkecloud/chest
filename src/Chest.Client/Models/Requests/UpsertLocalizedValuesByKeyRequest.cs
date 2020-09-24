using System.Collections.Generic;

namespace Chest.Client.Models.Requests
{
    public class UpsertLocalizedValuesByKeyRequest : UserRequest
    {
        /// <summary>
        /// Key of localized value
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Key in this dictionary represents a locale
        /// Value in this dictionary represents a translated value
        /// </summary>
        public IReadOnlyDictionary<string, string> Localizations { get; set; }
    }
}