using System.Collections.Generic;
using Chest.Client.Models.Responses.Chest.Client.Models.Responses;
using JetBrains.Annotations;

namespace Chest.Client.Models.Responses
{
    public class GetAllLocalizedValuesResponse : PaginatedResponseContract<LocalizedValueByKeyContract>
    {
        public GetAllLocalizedValuesResponse([NotNull] IReadOnlyList<LocalizedValueByKeyContract> contents, int start, int size,
            int totalSize) : base(contents, start, size, totalSize)
        {
        }
    }
}