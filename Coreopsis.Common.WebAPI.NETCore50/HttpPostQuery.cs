using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coreopsis.WebApi
{
    public sealed class HttpPostQuery<T> : AbstractHttpQuery<T>
    {       
        public HttpPostQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout) 
            : base(apiData, headers, connectionTimeout, apiQueryTimeout)
        {
        }

        public override T SendRequest()
        {
            string response = SendRequestString();

            return JsonSerializer.Deserialize<T>(response);
        }

        public override async Task<T> SendRequestAsync()
        {
            string response = await SendRequestStringAsync();

            return JsonSerializer.Deserialize<T>(response);
        }

        public override string SendRequestString()
        {
            HttpWebRequest request = CreateRequest();

            string response = GetResponse(request);

            return response;
        }

        public override async Task<string> SendRequestStringAsync()
        {
            HttpWebRequest request = CreateRequest();

            string response = await GetResponseAsync(request);

            return response;
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
