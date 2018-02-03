using System;
using System.Collections.Generic;

namespace Realmar.Pipes
{
    public interface IPipeResultReceiver
    {
        Action<IList<object>> Callback { set; }
        void AddResult(object result);
    }
}