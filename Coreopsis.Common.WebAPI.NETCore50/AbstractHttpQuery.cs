﻿using Coreopsis.Common.WebAPI.Exceptions;
using Coreopsis.Interfaces.WebApi;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Coreopsis.WebApi
{
    /// <summary>
    /// Класс выполнения запроса к API
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public abstract class AbstractHttpQuery<T> : IHttpQuery<T> where T : class
    {
        protected IApiData _apiData;

        protected WebHeaderCollection _headers;

        private TimeSpan _connectionTimeout;

        private TimeSpan _apiQueryTimeout;

        private IWebProxy _proxy;

        protected int _repeatRequestCountWithError;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiData"></param>
        /// <param name="headers"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="apiQueryTimeout"></param>
        /// <param name="proxy"></param>
        /// <param name="repeatRequestCountWithError">Количество повторных запросов при ошибке</param>
        public AbstractHttpQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout, IWebProxy proxy = null, int repeatRequestCountWithError = 0)
        {
            _apiData = apiData;
            _headers = headers;
            _connectionTimeout = connectionTimeout;
            _apiQueryTimeout = apiQueryTimeout;
            _proxy = proxy;
            _repeatRequestCountWithError = repeatRequestCountWithError;
        }

        public abstract T SendRequest(bool serilize);

        public abstract Task<T> SendRequestAsync(bool serilize);

        protected virtual void SetHeaders(HttpWebRequest request, WebHeaderCollection headers)
        {
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
                            request.Headers.Add(header, headers[header]);
                        }
                        break;
                }
            }
        }

        protected virtual string GetResponse(HttpWebRequest request)
        {
            string answer = null;
            int repeatCount = 0;

            WebException lastWebException = null;
            do
            {
                repeatCount++;
                try
                {
                    Thread.Sleep((int)_apiQueryTimeout.TotalMilliseconds);

                    request.Timeout = (int)_connectionTimeout.TotalMilliseconds;

                    if (_proxy != null)
                    {
                        request.Proxy = _proxy;
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8")))
                            {
                                string responseFromServer = reader.ReadToEnd();

                                answer = DecodeCharacters(responseFromServer);

                                return answer;
                            }
                        }
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        lastWebException = new NotImplementedWebAPIException(webEx.Message, webEx.Message, webEx);
                        continue;
                    }

                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        answer = Regex.Unescape(reader.ReadToEnd());

                        answer = answer.Replace("{", "").Replace("}", "");

                        answer = Regex.Replace(answer, "<[^>]+>", string.Empty).Replace("\\", "").Trim();
                    }

                    lastWebException = webEx;
                }
            }
            while (repeatCount < _repeatRequestCountWithError);

            if (lastWebException != null)
            {
                CreateException(answer, lastWebException, lastWebException.Response);
            }

            return null;
        }

        protected virtual async Task<string> GetResponseAsync(HttpWebRequest request)
        {
            string answer = null;

            int repeatCount = 0;

            WebException lastWebException = null;

            do
            {
                repeatCount++;
                try
                {
                    Thread.Sleep((int)_apiQueryTimeout.TotalMilliseconds);

                    request.Timeout = (int)_connectionTimeout.TotalMilliseconds;

                    if (_proxy != null)
                    {
                        request.Proxy = _proxy;
                    }

                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8")))
                            {
                                string responseFromServer = await reader.ReadToEndAsync();

                                answer = DecodeCharacters(responseFromServer);

                                return answer;
                            }
                        }
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        lastWebException = new NotImplementedWebAPIException(webEx.Message, webEx.Message, webEx);
                        continue;
                    }

                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        answer = Regex.Unescape(await reader.ReadToEndAsync());

                        answer = answer.Replace("{", "").Replace("}", "");

                        answer = Regex.Replace(answer, "<[^>]+>", string.Empty).Replace("\\", "").Trim();
                    }

                    lastWebException = webEx;
                }
            }
            while (repeatCount < _repeatRequestCountWithError);

            if (lastWebException != null)
            {
                CreateException(answer, lastWebException, lastWebException.Response);
            }

            return null;
        }
        private string DecodeCharacters(string text)
        {
            return Regex.Replace(
              text,
               @"\\\\u([\da-fA-F]{4})",
              m => {
                  return ((char)Int32.Parse(m.Groups[1].Value, NumberStyles.HexNumber)).ToString();
              }
            );
        }

        private void CreateException(string message, WebException exception, WebResponse response)
        {
            HttpWebResponse webResponse = response as System.Net.HttpWebResponse;

            if( webResponse == null)
            {
                throw new NotImplementedWebAPIException(message, exception.Message, exception);
            }

            switch (webResponse.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    {
                        throw new UnauthorizedException(message, exception.Message, exception);
                    }
                case HttpStatusCode.RequestTimeout:
                    {
                        throw new ReuqestTimeoutException(message, exception.Message, exception);
                    }
                case HttpStatusCode.BadRequest:
                    {
                        throw new Error400Exception(message, exception.Message, exception);
                    }
                case HttpStatusCode.Forbidden:
                    {
                        throw new Error403Exception(message, exception.Message, exception);
                    }
                case HttpStatusCode.NotFound:
                    {
                        throw new Error404Exception(message, exception.Message, exception);
                    }
                default:
                    {
                        throw new NotImplementedWebAPIException(message, exception.Message, exception);
                    }
            }
        }
    }
}
