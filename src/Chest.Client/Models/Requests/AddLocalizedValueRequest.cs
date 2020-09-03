using System.ComponentModel.DataAnnotations;

namespace Chest.Client.Models.Requests
{
    /// <summary>
    /// Request model to add localized value
    /// </summary>
    public class AddLocalizedValueRequest : UserRequest
    {
        /// <summary>
        /// Locale (e.g. en-US)
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Locale { get; set; }

        /// <summary>
        /// Key to translate
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Key { get; set; }

        /// <summary>
        /// Translated value
        /// </summary>
        [Required]
        [MaxLength(4096)]
        public string Value { get; set; }
    }
}