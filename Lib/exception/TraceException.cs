namespace Lib.Exception
{
    public class TraceException : System.Exception
    {
        public TraceException()
        {
        }

        public TraceException(string message)
            : base(message)
        {
        }

        public TraceException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}