using Coreopsis.Common.WebAPI.Exceptions;
using Coreopsis.Interfaces.WebApi;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coreopsis.WebApi
{
    public sealed class HttpPostQuery<T> : AbstractHttpQuery<T> where T : class
    {       
        public HttpPostQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout, IWebProxy proxy = null) 
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
            string data = _apiData.GetData();

            HttpWebRequest request = CreatePostRequest(uri, _headers);

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            byte[] bytes = encoding.GetBytes(data);
            request.ContentLength = data.Length;

            using (Stream streamWriter = request.GetRequestStream())
            {
                streamWriter.Write(bytes, 0, data.Length);
            }

            return request;
        }

        private HttpWebRequest CreatePostRequest(string uri, WebHeaderCollection headers)
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = WebRequestMethods.Http.Post;
            request.ProtocolVersion = HttpVersion.Version11;

            foreach (string header in headers)
            {
                switch (header)
                {
                    case "Connection":
                        {
                            request.Connection = headers[header];
                            
                        }
                        break;
                    case "ContentType":
                        {
                            request.ContentType = headers[header];
                            
                        }
                        break;
                    case "Host":
                        {
                            request.Host = headers[header];
                           
                        }
                        break;
                    case "UserAgent":
                        {
                            request.UserAgent = headers[header];
                            
                        }
                        break;
                    default:
                        {
                            request.Headers.Add(headers[header]);
                        }
                        break;
                }
            }
           
            return request;
        }
    }
}
