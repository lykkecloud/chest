// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Microsoft.OpenApi.Models;

namespace Chest
{
    using System;
    using System.Linq;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths
                .ToDictionary(
                    path => path.Key.Replace("v{version}", swaggerDoc.Info.Version, StringComparison.InvariantCulture),
                    path => path.Value);
            
            swaggerDoc.Paths = new OpenApiPaths();
            
            foreach (var openApiPathItem in paths)
            {
                swaggerDoc.Paths.Add(openApiPathItem.Key, openApiPathItem.Value);
            }
        }
    }
}
