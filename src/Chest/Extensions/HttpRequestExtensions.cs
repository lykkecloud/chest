// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Http;

namespace Chest.Extensions
{
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
