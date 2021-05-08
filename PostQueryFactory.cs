﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Coreopsis.WebApi
{
    public sealed class PostQueryFactory<T> : IHttpQueryFactory<T>
    {
        private IApiData _apiData;

        private Dictionary<HttpRequestHeader, string> _headerData;
        private Dictionary<string, string> _customHeaderData;

        private TimeSpan _connectionTimeout;

        private TimeSpan _apiQueryTimeout;

        public PostQueryFactory(
                IApiData apiData,
                string accept,
                string contentType,
                string userAgent,
                TimeSpan connectionTimeout,
                TimeSpan apiQueryTimeout)
        {
            _apiData = apiData;

            _customHeaderData = new Dictionary<string, string>();
            _headerData = new Dictionary<HttpRequestHeader, string>();
            _customHeaderData.Add("Accept", accept);
            _customHeaderData.Add("ContentType", contentType);
            _customHeaderData.Add("UserAgent", userAgent);

            _connectionTimeout = connectionTimeout;
            _apiQueryTimeout = apiQueryTimeout;
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

            return new HttpPostQuery<T>(_apiData, whc, _connectionTimeout, _apiQueryTimeout);
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