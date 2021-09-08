using System;
using System.Collections.Immutable;
using Lib.Result.Builder;

namespace Lib.Result
{
    [Serializable]
    public class TracedMethod
    {
        public static TracedMethodBuilder Builder()
        {
            return new TracedMethodBuilder();
        }

        public string MethodName { get; }
        public string ClassName { get; }
        public long ExecutionTime { get; }
        public ImmutableList<TracedMethod> TracedMethods { get; }
        
        private TracedMethod() {}

        public TracedMethod(string methodName, string className, long executionTime, ImmutableList<TracedMethod> tracedMethods)
        {
            MethodName = methodName;
            ClassName = className;
            ExecutionTime = executionTime;
            TracedMethods = tracedMethods;
        }

        public override string ToString()
        {
            return GetType().Name + "(" + MethodName + ", " + ClassName + ", " + ExecutionTime + ")";
        }
    }
}
