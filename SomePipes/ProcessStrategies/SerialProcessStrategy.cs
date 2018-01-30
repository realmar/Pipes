using SomePipes.Connector;

namespace SomePipes.ProcessStrategies
{
    public class SerialProcessStrategy : IProcessStrategy
    {
        public void Process<TIn>(IPipeConnector<TIn> firstConnector, TIn data)
        {
            firstConnector.Process(data);
        }

        public void Process<TIn>(IPipeConnector<TIn> firstConnector, TIn[] data)
        {
            foreach (var item in data)
            {
                Process(firstConnector, item);
            }
        }
    }
}