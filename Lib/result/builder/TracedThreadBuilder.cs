using System.Reflection;
using Lib.Exception;

namespace Lib.Result.Builder
{
    public class TracedThreadBuilder : ExecutableEntityBuilder
    {
        private int _id;
        
        public TracedThreadBuilder Id(int id)
        {
            _id = id;
            return this;
        }
        public override bool EndTracing(long time)
        {
            if (!IsTracing)
                throw new TraceException("Cannot end tracing: tracing not started.");
            if (CurrentlyTracingMethod == null)
                throw new TargetException("Cannot end tracing: tracing method is not specified.");

            if (CurrentlyTracingMethod.EndTracing(time))
            {
                TracedMethods.Add(CurrentlyTracingMethod.Build());
                CurrentlyTracingMethod = null;
                IsTracing = false;
                ExecutionTime += time - StartExecutionTime;
                return true;
            }

            return false;
        }

        public TracedThread Build()
        {
            if (IsTracing)
                throw new TraceException("Cannot build: trace is not stopped.");
            return new TracedThread(_id, ExecutionTime, TracedMethods.ToImmutable());
        }
    }
}