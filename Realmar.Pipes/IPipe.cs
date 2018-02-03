using System.Collections.Generic;
using Realmar.Pipes.Connector;
using Realmar.Pipes.ProcessStrategies;

namespace Realmar.Pipes
{
    public interface IPipe<TIn> : IPipeResultReceiver
    {
        IProcessStrategy ProcessStrategy { set; }
        IPipeConnector<TIn> FirstConnector { get; }

        void Process(IList<TIn> data);
    }
}