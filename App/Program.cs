using System;
using System.IO;
using System.Threading;
using Lib;
using App.Mapping;
using App.Utils;
using App.Writer;

namespace App
{
    public static class Program
    {
        private static ITracer _tracer;

        private static readonly string MESSAGE = "This program is a demonstration of the work of the tracer. " +
                                                 "Now tracing will be performed in two threads, in one of them " +
                                                 "methods will be called at the same level, as well as nested. " +
                                                 "Result will be written to console and to file (JSON and XML).";

        public static void Main()
        {
            Console.WriteLine(MESSAGE);
            string outPath = InputUtils.EnterString("path to output file");
            
            _tracer = new Tracer();
            Thread countThread = new Thread(Count);
            countThread.Start();

            _tracer.StartTrace();
            for (int i = 0; i < 1_000; i++)
                Console.WriteLine(i);
            _tracer.StopTrace();
            
            WriteResult(outPath);
        }

        private static void WriteResult(string outPath)
        {
            IMapper jsonMapper = new JsonMapper();
            IMapper xmlMapper = new XmlMapper();

            using (IWriter jsonWriter = new CustomWriter(new StreamWriter(outPath + ".json")))
            {
                jsonWriter.Write(jsonMapper, _tracer.GetTraceResult());
            }

            using (IWriter xmlWriter = new CustomWriter(new StreamWriter(outPath + ".xml")))
            {
                xmlWriter.Write(xmlMapper, _tracer.GetTraceResult());
            }

            using (IWriter consoleWriter = new CustomWriter(Console.Out))
            {
                consoleWriter.Write(xmlMapper.ToPrettyString(_tracer.GetTraceResult(), 4));
                consoleWriter.Write(jsonMapper.ToPrettyString(_tracer.GetTraceResult(), 4));
            }
        }

        private static void Count()
        {
            _tracer.StartTrace();
            NestedCount();
            for (int i = 0; i < 300; i++)
                Console.WriteLine(i);
            NestedCount();
            _tracer.StopTrace();
        }

        private static void NestedCount()
        {
            _tracer.StartTrace();
            for (int i = 0; i < 200; i++)
                Console.WriteLine(i);
            _tracer.StopTrace();
        }
    }
}