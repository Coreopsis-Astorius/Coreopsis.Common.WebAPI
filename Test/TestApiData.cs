using Coreopsis.Interfaces.WebApi;
using System;

namespace Test
{
    public  class TestApiData : IApiData
    {
        private string _uri;

        public TestApiData(string uri)
        {
            _uri = uri;
        }

        public string CreateUri()
        {
            return _uri;
        }

        public string GetData()
        {
            throw new NotImplementedException();
        }
    }
}
