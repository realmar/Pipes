using SomePipes.Connector;

namespace SomePipes.ProcessStrategies
{
    public interface IProcessStrategy
    {
        void Process<TIn>(IPipeConnector<TIn> firstConnector, TIn data);
        void Process<TIn>(IPipeConnector<TIn> firstConnector, TIn[] data);
    }
}