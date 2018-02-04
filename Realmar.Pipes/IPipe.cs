using System.Collections.Generic;
using Realmar.Pipes.Connector;

namespace Realmar.Pipes
{
    public interface IPipe<TIn> : IPipeResultReceiver
    {
        IPipeConnector<TIn> FirstConnector { get; }

        void Process(IList<TIn> data);
    }
}