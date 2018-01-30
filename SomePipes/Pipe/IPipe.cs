using SomePipes.Connector;
using SomePipes.ProcessStrategies;

namespace SomePipes.Pipe
{
    public interface IPipe<TIn>
    {
        IProcessStrategy ProcessStrategy { set; }
        IPipeConnector<TIn> FirstConnector { get; }

        void Process(TIn data);
    }
}