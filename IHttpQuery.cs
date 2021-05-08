using System.Threading.Tasks;

namespace Coreopsis.WebApi
{
    public interface IHttpQuery<T>
    {
        T SendRequest();
        Task<T> SendRequestAsync();
    }
}
