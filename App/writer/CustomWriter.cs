using System;
using System.IO;
using App.Mapping;

namespace App.Writer
{
    public class CustomWriter : IWriter
    {
        private readonly TextWriter _textWriter;
        
        public CustomWriter(TextWriter streamWriter)
        {
            if ((_textWriter = streamWriter) == null)
                throw new ArgumentNullException(nameof(streamWriter));
        }
        
        public void Write(IMapper mapper, object writable)
        {
            Write(mapper.ToString(writable));
        }

        public void Write(string writable)
        {
            _textWriter.Write(writable);
        }

        public void Dispose()
        {
            _textWriter.Dispose();
        }
    }
}