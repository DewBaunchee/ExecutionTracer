using System.Collections.Immutable;
using Lib.Exception;

namespace Lib.Result.Builder
{
    public abstract class ExecutableEntityBuilder
    {
        public bool IsTracing { protected set; get; }
        protected long StartExecutionTime;
        protected long ExecutionTime;

        protected readonly ImmutableList<TracedMethod>.Builder TracedMethods
            = ImmutableList.CreateBuilder<TracedMethod>();

        protected TracedMethodBuilder CurrentlyTracingMethod;

        public void StartTracing(long time, TracedMethodBuilder currentlyExecutingMethod)
        {
            if (!IsTracing)
            {
                IsTracing = true;
                StartExecutionTime = time;
            }

            if (CurrentlyTracingMethod == null)
            {
                CurrentlyTracingMethod = currentlyExecutingMethod;
                if (CurrentlyTracingMethod != null)
                    CurrentlyTracingMethod.StartTracing(time, null);
            }
            else
            {
                CurrentlyTracingMethod.StartTracing(time, currentlyExecutingMethod);
            }
        }

        public virtual bool EndTracing(long time)
        {
            if (!IsTracing)
                throw new TraceException("Cannot end tracing: tracing not started.");

            if (CurrentlyTracingMethod == null)
            {
                IsTracing = false;
                ExecutionTime = time - StartExecutionTime;
                return true;
            }

            if (CurrentlyTracingMethod.EndTracing(time))
            {
                TracedMethods.Add(CurrentlyTracingMethod.Build());
                CurrentlyTracingMethod = null;
            }

            return false;
        }
    }
}