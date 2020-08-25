namespace Chest.Client.Models
{
    /// <summary>
    /// Localized value
    /// </summary>
    public class LocalizedValueContract
    {
        /// <summary>
        /// Locale (e.g. en-US)
        /// </summary>
        public string Locale { get; set; }
        
        /// <summary>
        /// Key to translate
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Translated value
        /// </summary>
        public string Value { get; set; }
    }
}