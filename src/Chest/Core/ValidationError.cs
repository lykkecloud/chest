using System.Collections.Generic;
using System.Linq;

namespace Chest.Core
{
    public class ValidationError
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public IReadOnlyList<string> FieldNames { get; }

        public ValidationError(string errorCode, string errorMessage, IEnumerable<string> fieldNames = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            FieldNames = fieldNames == null ? new List<string>() : fieldNames.ToList(); 
        }
    }
}