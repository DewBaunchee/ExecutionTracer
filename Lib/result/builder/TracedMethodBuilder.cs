using Lib.Exception;

namespace Lib.Result.Builder
{
    public class TracedMethodBuilder : ExecutableEntityBuilder
    {
        private string _methodName;
        private string _className;

        public TracedMethodBuilder MethodName(string methodName)
        {
            _methodName = methodName;
            return this;
        }

        public TracedMethodBuilder ClassName(string className)
        {
            _className = className;
            return this;
        }

        public TracedMethod Build()
        {
            if (IsTracing)
                throw new TraceException("Cannot build: trace is not stopped.");
            return new TracedMethod(_methodName, _className, ExecutionTime, TracedMethods.ToImmutable());
        }
    }
}
