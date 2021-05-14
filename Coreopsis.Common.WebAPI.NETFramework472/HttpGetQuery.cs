using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

namespace Coreopsis.WebApi
{
    public class HttpGetQuery<T> : AbstractHttpQuery<T>
    {
        public HttpGetQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout) 
            : base(apiData, headers, connectionTimeout, apiQueryTimeout)
        {
        }

        public override T SendRequest()
        {
            HttpWebRequest request = CreateRequest();

            string response = GetResponse(request);

            return JsonSerializer.Deserialize<T>(response);
        }

        public override async Task<T> SendRequestAsync()
        {
            HttpWebRequest request = CreateRequest();

            string response = await GetResponseAsync(request);

            return JsonSerializer.Deserialize<T>(response);
        }

        private HttpWebRequest CreateRequest()
        {
            string uri = _apiData.CreateUri();

            HttpWebRequest request = CreateGetRequest(uri, _headers);

            return request;
        }

        private HttpWebRequest CreateGetRequest(string uri, WebHeaderCollection headers)
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = WebRequestMethods.Http.Get;

            request.ProtocolVersion = HttpVersion.Version11;

            request.Headers = headers;

            return request;
        }
    }
}
