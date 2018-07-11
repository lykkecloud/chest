// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtensions
    {
        public static Uri GetRelativeUrl(this HttpRequest request, string relativePath)
        {
            relativePath = relativePath.TrimStart('~');
            relativePath = relativePath.TrimStart('/');

            return new Uri($"{request.Scheme}://{request.Host}/{relativePath}");
        }
    }
}
