using System;
using System.Collections.Generic;
using SomePipes.Processors;

namespace SomePipes.Connector
{
    public interface IPipeConnector<TIn>
    {
        IPipeConnector<TOut> Connect<TOut>(IPipeProcessor<TIn, TOut> processor);
        void Finish(Action<IList<TIn>> callback);
        void Process(TIn data);
    }
}