namespace Chest.Client.Models.Responses
{
    /// <summary>
    /// Response model for get localized value by locale and key
    /// </summary>
    public class GetLocalizedValueResponse : ErrorCodeResponse<LocalizedValuesErrorCodesContract>
    {
        /// <summary>
        /// Localized value
        /// </summary>
        public LocalizedValueContract LocalizedValue { get; set; }
    }
}