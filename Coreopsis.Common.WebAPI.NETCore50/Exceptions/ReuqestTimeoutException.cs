using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class ReuqestTimeoutException : WebException
    {
        public string ServerError { get; private set; }
        public ReuqestTimeoutException(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
