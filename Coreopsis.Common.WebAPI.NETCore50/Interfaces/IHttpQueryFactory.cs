using System.Net;

namespace Coreopsis.Interfaces.WebApi
{
    public interface IHttpQueryFactory<T> where T : class
    {
        IHttpQuery<T> Create();

        void SetHeader(HttpRequestHeader header, string value);
        void SetHeader(string header, string value);
    }
}
