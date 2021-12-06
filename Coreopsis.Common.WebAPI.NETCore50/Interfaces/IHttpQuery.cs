using System.Threading.Tasks;

namespace Coreopsis.Interfaces.WebApi
{
    public interface IHttpQuery<T>  where T : class
    {
        T SendRequest(bool serilize);
        Task<T> SendRequestAsync(bool serilize);
    }
}
