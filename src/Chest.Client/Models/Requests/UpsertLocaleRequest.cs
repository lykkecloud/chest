using System.ComponentModel.DataAnnotations;

namespace Chest.Client.Models.Requests
{
    public class UpsertLocaleRequest : UserRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}