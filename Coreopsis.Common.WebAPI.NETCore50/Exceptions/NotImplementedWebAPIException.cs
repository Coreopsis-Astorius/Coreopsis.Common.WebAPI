using System.Net;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class NotImplementedWebAPIException : WebException
    {
        public string ServerError { get; private set; }
        public NotImplementedWebAPIException(string message, string returnedServerError, WebException exception)
            : base(message, exception)
        {
            ServerError = returnedServerError;
        }
    }
}
