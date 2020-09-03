using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Common.Model;

namespace Chest.Core
{
    public class ErrorResult<TError> : Result<TError> where TError : struct, Enum
    {
        public List<ValidationError> ValidationErrors { get; }

        public ErrorResult(TError error, IEnumerable<ValidationError> validationErrors) : base(error)
        {
            ValidationErrors = validationErrors.ToList();
        }
    }
}