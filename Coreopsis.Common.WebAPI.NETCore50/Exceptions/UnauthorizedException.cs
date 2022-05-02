using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class UnauthorizedException : WebException
    {
        public string ServerError { get; private set; }
        public UnauthorizedException(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
