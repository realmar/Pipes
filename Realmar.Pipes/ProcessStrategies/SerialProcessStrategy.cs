using System.Collections.Generic;
using Realmar.Pipes.Connector;

namespace Realmar.Pipes.ProcessStrategies
{
	public class SerialProcessStrategy : IProcessStrategy
	{
		public void Process<TIn>(IPipeConnector<TIn> pipeConnector, IList<TIn> data)
		{
			foreach (var item in data)
			{
				pipeConnector.Process(item);
			}
		}
	}
}