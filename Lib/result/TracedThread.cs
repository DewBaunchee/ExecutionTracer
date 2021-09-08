using System;
using System.Collections.Immutable;
using Lib.Result.Builder;

namespace Lib.Result
{
    [Serializable]
    public class TracedThread
    {
        public static TracedThreadBuilder Builder()
        {
            return new TracedThreadBuilder();
        } 
        public int Id { get; }
        public long ExecutionTime { get; }
        public ImmutableList<TracedMethod> TracedMethods { get; }
        
        private TracedThread() {}

        public TracedThread(int id, long executionTime, ImmutableList<TracedMethod> tracedMethods)
        {
            Id = id;
            ExecutionTime = executionTime;
            TracedMethods = tracedMethods;
        }

        public override string ToString()
        {
            return GetType().Name + "(" + Id + ", " + ExecutionTime + ")";
        }
    }
}