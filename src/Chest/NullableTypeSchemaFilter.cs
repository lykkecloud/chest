// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using System;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class NullableTypeSchemaFilter : ISchemaFilter
    {
        // NOTE (Rach): https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/498
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (context.SystemType.IsValueType && Nullable.GetUnderlyingType(context.SystemType) == null)
            {
                model.Extensions.Add("x-nullable", false);
            }
        }
    }
}