using System.Collections.Generic;
using Realmar.Pipes.Connector;

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
		public void Process<TIn>(IPipeConnector<TIn> pipeConnector, IList<TIn> data)
		{
			foreach (var item in data)
			{
				pipeConnector.Process(item);
			}
		}
	}
}