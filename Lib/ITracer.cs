using Lib.Result;

namespace Lib
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        bool IsTracing();
        void Clear();
        TraceResult GetTraceResult();
    }
}
