using System.Collections.Generic;
using System.Threading.Tasks;
using Realmar.Pipes.Connector;

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
		public void Process<TIn>(IPipeConnector<TIn> pipeConnector, IList<TIn> data)
		{
			Parallel.ForEach(data, pipeConnector.Process);
		}
	}
}