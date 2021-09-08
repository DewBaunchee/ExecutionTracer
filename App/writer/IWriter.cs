using System;
using App.Mapping;

namespace App.Writer
{
    public interface IWriter : IDisposable
    {

        void Write(IMapper mapper, object writable);
        
        void Write(string writable);
    }
}