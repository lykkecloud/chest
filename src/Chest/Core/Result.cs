using System;
using System.Collections.Generic;
using Lykke.Snow.Common.Model;

namespace Chest.Core
{
    public class ErrorResult<TError> : Result<TError> where TError : struct, Enum
    {
        public string Message { get; }
        public IEnumerable<string> Fields { get; }

        public ErrorResult(TError error, string message = null, IEnumerable<string> fields = null) : base(error)
        {
            Message = message;
            Fields = fields ?? new List<string>();
        }

        // hack
        public ErrorResult(Result<TError> result) : base(result.Error)
        {
            Fields = new List<string>();
        }

        public List<ValidationError> ToValidationErrors()
        {
            var error = new ValidationError(Error.ToString(), Message, Fields);
            return new List<ValidationError>()
            {
                error,
            };
        }
    }
}