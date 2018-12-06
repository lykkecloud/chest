// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using System.Linq;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "version");
            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }
}
