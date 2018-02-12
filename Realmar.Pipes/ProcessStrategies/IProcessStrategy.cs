using System.Collections.Generic;
using Realmar.Pipes.Connector;

namespace Realmar.Pipes.ProcessStrategies
{
    public interface IProcessStrategy
    {
        void Process<TIn>(IPipeConnector<TIn> pipeConnector, IList<TIn> data);
    }
}