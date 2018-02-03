using System.Collections.Generic;

namespace Realmar.Pipes.ProcessStrategies
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