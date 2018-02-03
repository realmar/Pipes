using System.Collections.Generic;
using SomePipes.Pipe;

namespace SomePipes.ProcessStrategies
{
    public class SerialProcessStrategy : IProcessStrategy
    {
        public void Process<TIn>(IPipe<TIn> pipe, IList<TIn> data)
        {
            foreach (var item in data)
            {
                pipe.FirstConnector.Process(item);
            }
        }
    }
}