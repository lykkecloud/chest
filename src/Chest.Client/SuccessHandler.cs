// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chest.Client
{
    public class SuccessHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var contentMessage = default(string);
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                contentMessage = JsonConvert.DeserializeObject<Content>(content)?.Message;

                response.Content.Dispose();
            }

            throw new HttpException(
                response.RequestMessage.Method,
                response.RequestMessage.RequestUri,
                response.StatusCode,
                response.ReasonPhrase,
                contentMessage);
        }

        private class Content
        {
            public string Message { get; set; }
        }
    }
}
