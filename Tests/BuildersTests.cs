using System;
using Lib.Result;
using Lib.Result.Builder;
using NUnit.Framework;

namespace Tests
{
    public class BuildersTests
    {
        [Test]
        public void TraceResultBuilder()
        {
            TraceResult traceResult = TraceResult.Builder()
                .AddTracedThread(new TracedThread(12, 4, null))
                .Build();
            TracedThread addedThread = traceResult.TracedThreads[0];
            Assert.AreEqual(12, addedThread.Id);
            Assert.AreEqual(4, addedThread.ExecutionTime);
            Assert.AreEqual(null, addedThread.TracedMethods);
        }

        [Test]
        public void TracedThreadBuilderWithoutTracedMethod()
        {
            TracedThreadBuilder builder = TracedThread.Builder()
                .Id(12);
            builder.StartTracing(4120, null);
            
            Assert.True(builder.IsTracing);
            Assert.Catch<Exception>(() => builder.EndTracing(10000));
            Assert.Catch<Exception>(() => builder.Build());
        }

        [Test]
        public void TracedThreadBuilder()
        {
            TracedThreadBuilder builder = TracedThread.Builder()
                .Id(12);
            builder.StartTracing(4120, TracedMethod.Builder()
                .ClassName("Program")
                .MethodName("Main")
            );
            
            Assert.True(builder.IsTracing);
            Assert.True(builder.EndTracing(10000));
            
            TracedThread thread = builder.Build();
            Assert.AreEqual(10000 - 4120, thread.ExecutionTime);
            Assert.AreEqual(1,thread.TracedMethods.Count);
            Assert.AreEqual(12, thread.Id);

            TracedMethod method = thread.TracedMethods[0];
            Assert.AreEqual("Program", method.ClassName);
            Assert.AreEqual("Main", method.MethodName);
        }

        [Test]
        public void TracedMethodBuilder()
        {
            TracedMethodBuilder builder = TracedMethod.Builder()
                .ClassName("Program")
                .MethodName("Main");
            builder.StartTracing(4120, null);
            
            Assert.True(builder.IsTracing);
            Assert.True(builder.EndTracing(10000));
            
            TracedMethod method = builder.Build();
            Assert.AreEqual(10000 - 4120, method.ExecutionTime);
            Assert.AreEqual(0, method.TracedMethods.Count);
            Assert.AreEqual("Program", method.ClassName);
            Assert.AreEqual("Main", method.MethodName);
        }
    }
}