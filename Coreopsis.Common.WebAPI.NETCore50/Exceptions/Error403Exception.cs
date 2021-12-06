using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class Error403Exception : WebException
    {
        public string ServerError { get; private set; }
        public Error403Exception(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
