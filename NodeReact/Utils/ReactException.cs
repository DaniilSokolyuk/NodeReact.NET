using System;

namespace NodeReact.Utils
{
    public class NodeReactException : Exception
    {
        public NodeReactException(string message) : base(message)
        {

        }

        public NodeReactException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
