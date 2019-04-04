// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

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
