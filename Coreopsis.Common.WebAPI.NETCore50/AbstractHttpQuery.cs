﻿using System;
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
    public abstract class AbstractHttpQuery<T> : IHttpQuery<T>
    {
        protected IApiData _apiData;

        protected WebHeaderCollection _headers;

        private TimeSpan _connectionTimeout;

        private TimeSpan _apiQueryTimeout;

        public AbstractHttpQuery(IApiData apiData, WebHeaderCollection headers, TimeSpan connectionTimeout, TimeSpan apiQueryTimeout)
        {
            _apiData = apiData;
            _headers = headers;
            _connectionTimeout = connectionTimeout;
            _apiQueryTimeout = apiQueryTimeout;
        }

        public abstract T SendRequest();

        public abstract Task<T> SendRequestAsync();

        public abstract string SendRequestString();

        public abstract Task<string> SendRequestStringAsync();

        protected virtual string GetResponse(HttpWebRequest request)
        {
            string answer = null;

            try
            {
                Thread.Sleep((int)_apiQueryTimeout.TotalMilliseconds);

                request.Timeout = (int)_connectionTimeout.TotalMilliseconds;

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
                    throw new WebException(webEx.Message, webEx);
                }
                
                using (var stream = webEx.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    answer = Regex.Unescape(reader.ReadToEnd());

                    answer = answer.Replace("{", "").Replace("}", "");

                    answer = Regex.Replace(answer, "<[^>]+>", string.Empty).Replace("\\", "").Trim();
                }

                throw new WebException(answer, webEx);
            }
        }

        protected virtual async Task<string> GetResponseAsync(HttpWebRequest request)
        {
            string answer = null;

            try
            {
                Thread.Sleep((int)_apiQueryTimeout.TotalMilliseconds);

                request.Timeout = (int)_connectionTimeout.TotalMilliseconds;

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
                    throw new WebException(webEx.Message, webEx);
                }

                using (var stream = webEx.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    answer = Regex.Unescape(await reader.ReadToEndAsync());

                    answer = answer.Replace("{", "").Replace("}", "");

                    answer = Regex.Replace(answer, "<[^>]+>", string.Empty).Replace("\\", "").Trim();
                }

                throw new WebException(answer, webEx);
            }
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
    }
}
