﻿using System.Collections.Generic;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes
{
	/// <summary>
	/// The IPipe interface.
	/// Represents a pipe which processes data using a set of <see cref="IPipeProcessor{TIn,TOut}"/>s.
	/// </summary>
	/// <typeparam name="TIn">The type of the input data.</typeparam>
	/// <seealso cref="IResultReceiver" />
	public interface IPipe<TIn> : IResultReceiver
	{
		/// <summary>
		/// Gets the first connector.
		/// </summary>
		/// <value>
		/// The first connector.
		/// </value>
		IProcessorConnector<TIn> FirstConnector { get; }

		/// <summary>
		/// Processes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		void Process(IList<TIn> data);
	}
}