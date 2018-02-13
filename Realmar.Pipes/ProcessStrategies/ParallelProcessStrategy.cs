using System.Collections.Generic;
using System.Threading.Tasks;
using Realmar.Pipes.Connectors;

namespace Realmar.Pipes.ProcessStrategies
{
	/// <summary>
	/// The ParallelProcessStrategy.
	/// Processes the specified data in parallel using <code>Parallel.ForEach</code>.
	/// </summary>
	/// <seealso cref="Realmar.Pipes.ProcessStrategies.IProcessStrategy" />
	public class ParallelProcessStrategy : IProcessStrategy
	{
		/// <inheritdoc />
		public void Process<TIn>(IProcessorConnector<TIn> processorConnector, IList<TIn> data)
		{
			Parallel.ForEach(data, processorConnector.Process);
		}
	}
}