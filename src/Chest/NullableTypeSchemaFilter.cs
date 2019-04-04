// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

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