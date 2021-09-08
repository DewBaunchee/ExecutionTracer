using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Lib.Exception;
using Lib.Result;
using Lib.Result.Builder;
using Lib.Utils;

namespace Lib
{
    public class Tracer : ITracer
    {
        private readonly Dictionary<int, TracedThreadBuilder> _tracedThreadBuilders =
            new Dictionary<int, TracedThreadBuilder>();

        private TraceResult _cachedResult;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartTrace()
        {
            _cachedResult = null;
            var startTracing = TimeUtils.GetNowTime();
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            TracedThreadBuilder tracedThreadBuilder;
            if (_tracedThreadBuilders.ContainsKey(currentThreadId))
            {
                tracedThreadBuilder = _tracedThreadBuilders[currentThreadId];
            }
            else
            {
                tracedThreadBuilder = TracedThread.Builder().Id(currentThreadId);
                _tracedThreadBuilders[currentThreadId] = tracedThreadBuilder;
            }

            var stackTrace = new StackTrace();
            var method = stackTrace.GetFrame(1)?.GetMethod();
            if (method == null)
                throw new TraceException("Cannot start trace: Method base is null.");

            tracedThreadBuilder.StartTracing(
                startTracing,
                TracedMethod.Builder()
                    .ClassName(method.DeclaringType?.Name)
                    .MethodName(method.Name)
            );
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StopTrace()
        {
            _cachedResult = null;
            var endTracing = TimeUtils.GetNowTime();
            var currentThreadId = Thread.CurrentThread.ManagedThreadId;

            if (!_tracedThreadBuilders.ContainsKey(currentThreadId))
                throw new TraceException("Cannot stop trace: tracing is not started.");
            var tracedThreadBuilder = _tracedThreadBuilders[currentThreadId];

            tracedThreadBuilder.EndTracing(endTracing);
        }

        public bool IsTracing()
        {
            foreach (var tracedThreadBuilder in _tracedThreadBuilders.Values)
                if (tracedThreadBuilder.IsTracing)
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            _cachedResult = null;
            _tracedThreadBuilders.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TraceResult GetTraceResult()
        {
            if (_cachedResult != null)
                return _cachedResult;
            
            var traceResultBuilder = TraceResult.Builder();

            foreach (var tracingThread in _tracedThreadBuilders)
                if (!tracingThread.Value.IsTracing)
                    traceResultBuilder.AddTracedThread(tracingThread.Value.Build());
            
            return _cachedResult = traceResultBuilder.Build();
        }
    }
}