using System.ComponentModel.DataAnnotations;

namespace Chest.Client.Models.Requests
{
    /// <summary>
    /// Request model to update localized value
    /// </summary>
    public class UpdateLocalizedValueRequest : UserRequest
    {
        /// <summary>
        /// Translated value
        /// </summary>
        [Required]
        [MaxLength(4096)]
        public string Value { get; set; }
    }
}