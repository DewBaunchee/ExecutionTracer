using System;
using System.Collections.Immutable;
using Lib.Result.Builder;

namespace Lib.Result
{
    [Serializable]
    public class TraceResult
    {
        public static TraceResultBuilder Builder()
        {
            return new TraceResultBuilder();
        }

        public ImmutableList<TracedThread> TracedThreads { get; }

        private TraceResult() {}
        
        public TraceResult(ImmutableList<TracedThread> tracedThreads)
        {
            TracedThreads = tracedThreads;
        }

        public override string ToString()
        {
            return GetType().Name + "(" +  String.Join(", ", TracedThreads) + ")";
        }
    }
}