using System.Collections.Generic;

namespace Chest.Client.Models.Responses
{
    public class ErrorsResponse
    {
        public IReadOnlyList<ValidationErrorContract> Errors { get; set; } = new List<ValidationErrorContract>();
    }
}