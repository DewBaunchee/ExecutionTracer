using System.Linq;
using System.Threading;
using Lib;
using Lib.Result;
using NUnit.Framework;

namespace Tests
{
    public class TracerTests
    {
        private ITracer _tracer;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _tracer = new Tracer();
        }

        [SetUp]
        public void BeforeEach()
        {
            _tracer.Clear();
        }

        [Test]
        public void OneThreadTracingTest()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();

            TraceResult traceResult = _tracer.GetTraceResult();
            Assert.AreEqual(1, traceResult.TracedThreads.Count);
            
            TracedThread tracedThread = traceResult.TracedThreads[0];
            Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, tracedThread.Id);
            Assert.AreEqual(1, tracedThread.TracedMethods.Count);
            Assert.LessOrEqual(100, tracedThread.ExecutionTime);

            TracedMethod tracedMethod = tracedThread.TracedMethods[0];
            Assert.AreEqual(GetType().Name, tracedMethod.ClassName);
            Assert.AreEqual("OneThreadTracingTest", tracedMethod.MethodName);
            Assert.AreEqual(0,tracedMethod.TracedMethods.Count);
            Assert.LessOrEqual(100, tracedMethod.ExecutionTime);
        }

        [Test]
        public void MultiThreadTracingTest()
        {
            Thread thread = new Thread(Sleep);
            thread.Start();
            
            _tracer.StartTrace();
            Thread.Sleep(400);
            _tracer.StopTrace();

            TraceResult traceResult = _tracer.GetTraceResult();
            Assert.AreEqual(2, traceResult.TracedThreads.Count);
            
            TracedThread[] tracedThreads = traceResult.TracedThreads
                .Where(value => value.Id == thread.ManagedThreadId).ToArray();
            Assert.AreEqual(1, tracedThreads.Length);

            TracedThread secondaryThread = tracedThreads[0];
            Assert.AreEqual(1, secondaryThread.TracedMethods.Count);
            Assert.LessOrEqual(200, secondaryThread.ExecutionTime);

            TracedMethod tracedMethodInSecondary = secondaryThread.TracedMethods[0];
            Assert.AreEqual(GetType().Name, tracedMethodInSecondary.ClassName);
            Assert.AreEqual("Sleep", tracedMethodInSecondary.MethodName);
            Assert.AreEqual(2, tracedMethodInSecondary.TracedMethods.Count);
            Assert.LessOrEqual(200, tracedMethodInSecondary.ExecutionTime);

            TracedMethod nestedTracedMethodInSecondary = tracedMethodInSecondary.TracedMethods[0];
            Assert.AreEqual(GetType().Name, nestedTracedMethodInSecondary.ClassName);
            Assert.AreEqual("NestedSleep", nestedTracedMethodInSecondary.MethodName);
            Assert.AreEqual(nestedTracedMethodInSecondary.TracedMethods.Count, 0);
            Assert.LessOrEqual(100, nestedTracedMethodInSecondary.ExecutionTime);

            
            TracedThread primaryThread = traceResult.TracedThreads
                .Where(value => value.Id == Thread.CurrentThread.ManagedThreadId).ToArray()[0];
            Assert.AreEqual(1, primaryThread.TracedMethods.Count);
            Assert.LessOrEqual(400, primaryThread.ExecutionTime);

            TracedMethod tracedMethodInPrimary = primaryThread.TracedMethods[0];
            Assert.AreEqual(GetType().Name, tracedMethodInPrimary.ClassName);
            Assert.AreEqual("MultiThreadTracingTest", tracedMethodInPrimary.MethodName);
            Assert.LessOrEqual(400, tracedMethodInPrimary.ExecutionTime);
        }

        private void Sleep()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            NestedSleep();
            NestedSleep();
            _tracer.StopTrace();
        }

        private void NestedSleep()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }
    }
}