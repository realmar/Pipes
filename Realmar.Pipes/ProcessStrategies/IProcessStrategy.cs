using System.Collections.Generic;
using Realmar.Pipes.Connectors;

namespace Realmar.Pipes.ProcessStrategies
{
	/// <summary>
	/// The IProcessStrategy interface.
	/// Defines how the specified data is processed.
	/// </summary>
	public interface IProcessStrategy
	{
		/// <summary>
		/// Processes the specified data using a pipe connector as starting point.
		/// </summary>
		/// <typeparam name="TIn">The type of the input data.</typeparam>
		/// <param name="processorConnector">The pipe connector.</param>
		/// <param name="data">The data.</param>
		void Process<TIn>(IProcessorConnector<TIn> processorConnector, IList<TIn> data);
	}
}