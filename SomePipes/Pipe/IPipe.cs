using System.Collections.Generic;
using SomePipes.Connector;
using SomePipes.ProcessStrategies;

namespace SomePipes.Pipe
{
    public interface IPipe<TIn> : IPipeResultReceiver
    {
        IProcessStrategy ProcessStrategy { set; }
        IPipeConnector<TIn> FirstConnector { get; }

        void Process(IList<TIn> data);
    }
}