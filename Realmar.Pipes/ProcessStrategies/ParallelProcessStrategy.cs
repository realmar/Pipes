using System.Collections.Generic;
using System.Threading.Tasks;
using Realmar.Pipes.Connector;

namespace Realmar.Pipes.ProcessStrategies
{
	public class ParallelProcessStrategy : IProcessStrategy
	{
		public void Process<TIn>(IPipe<TIn> pipe, TIn data)
		{
			pipe.FirstConnector.Process(data);
		}

		public void Process<TIn>(IPipeConnector<TIn> pipeConnector, IList<TIn> data)
		{
			Parallel.ForEach(data, pipeConnector.Process);

		}
	}
}