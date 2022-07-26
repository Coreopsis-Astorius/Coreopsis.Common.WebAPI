using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class Error400Exception : WebException
    {
        public string ServerError { get; private set; }
        public Error400Exception(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
