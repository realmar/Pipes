using System.Collections.Generic;
using Realmar.Pipes.Connectors;

namespace Realmar.Pipes.ProcessStrategies
{
	/// <summary>
	/// The SerialProcessStrategy.
	/// Processes the specified data in serial.
	/// </summary>
	/// <seealso cref="Realmar.Pipes.ProcessStrategies.IProcessStrategy" />
	public class SerialProcessStrategy : IProcessStrategy
	{
		/// <inheritdoc />
		public void Process<TIn>(IProcessorConnector<TIn> processorConnector, IList<TIn> data)
		{
			foreach (var item in data)
			{
				processorConnector.Process(item);
			}
		}
	}
}