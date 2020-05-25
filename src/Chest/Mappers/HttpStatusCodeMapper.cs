// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Mappers
{
    using System;
    using System.Net;
    using Exceptions;
    using Lykke.Middlewares.Mappers;

    public class HttpStatusCodeMapper : IHttpStatusCodeMapper
    {
        public HttpStatusCode Map(Exception exception)
        {
            switch (exception)
            {
                case DuplicateKeyException _:
                    return HttpStatusCode.Conflict;
                case NotFoundException _:
                    return HttpStatusCode.NotFound;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}