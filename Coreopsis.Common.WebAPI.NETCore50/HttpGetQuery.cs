using Coreopsis.Interfaces.WebApi;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coreopsis.WebApi
{
    public class HttpGetQuery<T> : AbstractHttpQuery<T> where T : class
    {
        public HttpGetQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout, IWebProxy proxy = null) 
            : base(apiData, headers, connectionTimeout, apiQueryTimeout, proxy)
        {
        }

        public override T SendRequest(bool serilize)
        {
            HttpWebRequest request = CreateRequest();

            string response = GetResponse(request);

            if (serilize)
            {
                return JsonSerializer.Deserialize<T>(response);
            }

            if (response as T != null)
            {
                return response as T;
            }

            throw new SerializationException("Destination serialization object is not a `String`");
        }

        public override async Task<T> SendRequestAsync(bool serilize)
        {
            HttpWebRequest request = CreateRequest();

            string response = await GetResponseAsync(request);

            if (serilize)
            {
                return JsonSerializer.Deserialize<T>(response);
            }

            if (response as T != null)
            {
                return response as T;
            }

            throw new SerializationException("Destination serialization object is not a `String`");
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
