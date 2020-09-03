using System.ComponentModel.DataAnnotations;

namespace Chest.Client.Models.Requests
{
    public class UserRequest
    {
        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}