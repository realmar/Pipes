using System;
using System.Collections.Generic;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes
{
	/// <summary>
	/// The IResultReceiver interface.
	/// Receives the processed data of the last <see cref="IPipeProcessor{TIn,TOut}"/> in the chain.
	/// </summary>
	public interface IResultReceiver
	{
		/// <summary>
		/// Sets the callback which is invoked by an
		/// <see cref="IPipe{TIn}"/> after processing the input data.
		/// </summary>
		/// <value>
		/// The callback.
		/// </value>
		Action<IList<object>> Callback { set; }

		/// <summary>
		/// Adds the final result after processing the data.
		/// </summary>
		/// <param name="result">The result.</param>
		void AddResult(object result);
	}
}