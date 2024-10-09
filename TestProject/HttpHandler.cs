using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpHandler
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, (HttpStatusCode StatusCode, object ResponseBody)> _mockResponses
            = new Dictionary<string, (HttpStatusCode, object)>();

        public void When(string url, HttpStatusCode statusCode, object responseBody)
        {
            _mockResponses[url] = (statusCode, responseBody);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && _mockResponses.TryGetValue(request.RequestUri.ToString(), out var mockResponse))
            {
                var response = new HttpResponseMessage(mockResponse.StatusCode)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(mockResponse.ResponseBody))
                };
                return Task.FromResult(response);
            }

            // Return a 404 if no mock response is found
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}