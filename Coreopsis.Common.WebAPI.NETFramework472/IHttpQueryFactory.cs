using System.Net;

namespace Coreopsis.WebApi
{
    public interface IHttpQueryFactory<T>
    {
        IHttpQuery<T> Create();

        void SetHeader(HttpRequestHeader header, string value);
        void SetHeader(string header, string value);
    }
}
