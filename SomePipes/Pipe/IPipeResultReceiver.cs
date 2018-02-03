using System;
using System.Collections.Generic;

namespace SomePipes.Pipe
{
    public interface IPipeResultReceiver
    {
        Action<IList<object>> Callback { set; }
        void AddResult(object result);
    }
}