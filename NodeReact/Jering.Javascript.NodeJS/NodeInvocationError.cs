using System.Text.Json.Serialization;

namespace NodeReact
{
    public class NodeInvocationError
    {
        public string ErrorMessage { get; set; }

        public string ErrorStack { get; set; }
    }
}
