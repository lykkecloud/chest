using System.Collections.Generic;

namespace Chest.Client.Models.Responses
{
    /// <summary>
    /// Response model to get all localized values by locale
    /// </summary>
    public class GetLocalizedValuesByLocaleResponse
    {
        /// <summary>
        /// Localized values as a flat object
        /// </summary>
        public IReadOnlyDictionary<string, string> LocalizedValues { get; set; }
    }
}