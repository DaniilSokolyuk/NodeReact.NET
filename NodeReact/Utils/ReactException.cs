using System;

namespace NodeReact.Utils
{
    public class ZeroReactException : Exception
    {
        public ZeroReactException(string message) : base(message)
        {

        }

        public ZeroReactException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
