using System.Collections.Generic;

namespace Chest.Client.Models.Responses
{
    public class GetLocalesResponse 
    {
        public IReadOnlyList<LocaleContract> Locales { get; set; }
    }
}