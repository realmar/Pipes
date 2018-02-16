using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Connectors;

namespace Realmar.Pipes.ProcessStrategies
{
	/// <summary>
	/// The ThreadPoolProcessStrategy class.
	/// Process strategy which uses a ThreadPool to execute each item
	/// in a separate thread. This strategy does not block when given data.
	/// </summary>
	/// <seealso cref="Realmar.Pipes.ProcessStrategies.IProcessStrategy" />
	public class ThreadPoolProcessStrategy : IProcessStrategy
	{
		/// <summary>
		/// Processes the specified data using a pipe connector as starting point.
		/// Does not block.
		/// </summary>
		/// <typeparam name="TIn">The type of the input data.</typeparam>
		/// <param name="processorConnector">The pipe connector.</param>
		/// <param name="data">The data.</param>
		public void Process<TIn>(IProcessorConnector<TIn> processorConnector, IList<TIn> data)
		{
			foreach (var item in data)
			{
				ThreadPool.QueueUserWorkItem(obj => processorConnector.Process(item));
			}
		}
	}
}