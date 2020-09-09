using System.Collections.Generic;

namespace Chest.Client.Models.Responses
{
    public class UpsertLocaleErrorCodeResponse : ErrorCodeResponse<LocalesErrorCodesContract>
    {
        /// <summary>
        ///  If we try to make a locale default, it should contain all localized values beforehand
        /// This list contains keys that need to be localized
        /// </summary>
        public IReadOnlyDictionary<string, string> Errors { get; set; }
    }
}