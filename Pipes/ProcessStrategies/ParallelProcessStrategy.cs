using System.Collections.Generic;
using System.Threading.Tasks;

namespace Realmar.Pipes.ProcessStrategies
{
    public class ParallelProcessStrategy : IProcessStrategy
    {
        public void Process<TIn>(IPipe<TIn> pipe, TIn data)
        {
            pipe.FirstConnector.Process(data);
        }

        public void Process<TIn>(IPipe<TIn> pipe, IList<TIn> data)
        {
            var tasks = new List<Task>();
            foreach (var item in data)
            {
                tasks.Add(Task.Run(() => pipe.FirstConnector.Process(item)));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}