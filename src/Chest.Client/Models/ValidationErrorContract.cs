using System.Collections.Generic;
using System.Linq;

namespace Chest.Client.Models
{
    public class ValidationErrorContract
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public IReadOnlyList<string> FieldNames { get; }

        public ValidationErrorContract(string errorCode, string errorMessage, IEnumerable<string> fieldNames = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            FieldNames = fieldNames?.ToList() ?? new List<string>();
        }
    }
}