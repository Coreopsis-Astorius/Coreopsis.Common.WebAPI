using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class Error404Exception : WebException
    {
        public string ServerError { get; private set; }
        public Error404Exception(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
