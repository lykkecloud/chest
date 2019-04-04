// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Mappers
{
    using System;
    using System.Net;
    using Chest.Exceptions;
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