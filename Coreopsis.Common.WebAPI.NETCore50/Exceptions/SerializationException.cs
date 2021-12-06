using System;

namespace Coreopsis.Common.WebAPI.Exceptions
{
    public class SerializationException : Exception
    {
        public SerializationException(string message) : base(message)
        {

        }
    }
}
