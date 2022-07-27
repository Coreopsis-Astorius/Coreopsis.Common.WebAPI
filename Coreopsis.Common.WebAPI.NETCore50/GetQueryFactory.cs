using Coreopsis.Interfaces.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Coreopsis.WebApi
{
    public sealed class GetQueryFactory<T> : IHttpQueryFactory<T> where T : class
    {
        private IApiData _apiData;
        private TimeSpan _connectionTimeout;
        private TimeSpan _apiQueryTimeout;

        private Dictionary<HttpRequestHeader, string> _headerData;
        private Dictionary<string, string> _customHeaderData;
        
        private IWebProxy _proxy;

        protected int _repeatRequestCountWithError;
        public GetQueryFactory(
                IApiData apiData,
                string contentType,
                string userAgent,
                TimeSpan connectionTimeout,
                TimeSpan apiQueryTimeout,
                IWebProxy proxy = null,
                int repeatRequestCountWithError = 0)
        {
            _apiData = apiData;

            _headerData = new Dictionary<HttpRequestHeader, string>();
            _customHeaderData = new Dictionary<string, string>();

            _headerData.Add(HttpRequestHeader.ContentType, contentType);
            _headerData.Add(HttpRequestHeader.UserAgent, userAgent);

            _connectionTimeout = connectionTimeout;
            _apiQueryTimeout = apiQueryTimeout;
            _proxy = proxy;
            _repeatRequestCountWithError = repeatRequestCountWithError;
        }

        public IHttpQuery<T> Create()
        {
            WebHeaderCollection whc = new WebHeaderCollection();

            foreach (var header in _headerData.Where(val => !string.IsNullOrEmpty(val.Value)))
            {
                whc.Add(header.Key, header.Value);
            }

            foreach (var header in _customHeaderData.Where(val => !string.IsNullOrEmpty(val.Value)))
            {
                whc.Add(header.Key, header.Value);
            }

            return new HttpGetQuery<T>(_apiData, whc, _connectionTimeout, _apiQueryTimeout, _proxy, _repeatRequestCountWithError);
        }

        public void SetHeader(HttpRequestHeader header, string value)
        {
            if (!_headerData.ContainsKey(header))
            {
                _headerData.Add(header, value);
            }
        }

        public void SetHeader(string header, string value)
        {
            if (!_customHeaderData.ContainsKey(header))
            {
                _customHeaderData.Add(header, value);
            }
        }
    }
}
