using System;
using System.Collections.Generic;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Connector
{
	/// <summary>
	/// The IPipeConnector interface.
	/// Stores a <see cref="IPipeProcessor{TIn,TOut}"/> and connects
	/// it to the next <see cref="IPipeProcessor{TIn,TOut}"/> in the chain.
	/// </summary>
	/// <typeparam name="TIn">The type of the input data.</typeparam>
	public interface IPipeConnector<TIn>
	{
		/// <summary>
		/// Connects the specified processor.
		/// </summary>
		/// <typeparam name="TOut">The type of the output data.</typeparam>
		/// <param name="processor">The processor to connect.</param>
		/// <returns>The next <see cref="IPipeConnector{TIn}"/> in the chain.</returns>
		IPipeConnector<TOut> Connect<TOut>(IPipeProcessor<TIn, TOut> processor);

		/// <summary>
		/// Finishes the chain of <see cref="IPipeProcessor{TIn,TOut}"/>s.
		/// </summary>
		/// <param name="callback">The callback to be executed when the data is processed.</param>
		void Finish(Action<IList<TIn>> callback);

		/// <summary>
		/// Processes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		void Process(TIn data);
	}
}