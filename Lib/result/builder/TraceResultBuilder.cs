using System.Collections.Immutable;

namespace Lib.Result.Builder
{
    public class TraceResultBuilder
    {
        private readonly ImmutableList<TracedThread>.Builder _tracedThreads = ImmutableList.CreateBuilder<TracedThread>();

        public TraceResultBuilder AddTracedThread(TracedThread tracedThread)
        {
            _tracedThreads.Add(tracedThread);
            return this;
        }

        public TraceResult Build()
        {
            return new TraceResult(_tracedThreads.ToImmutable());
        }
    }
}