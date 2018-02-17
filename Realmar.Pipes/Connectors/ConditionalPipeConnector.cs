using System;
using System.Collections.Generic;

namespace Realmar.Pipes.Connectors
{
	/// <summary>
	/// The ConditionalPipeConnector class.
	/// Takes data as input and uses a predicate to determine to which pipe
	/// it should give the data for further processing.
	/// </summary>
	/// <typeparam name="T">The input data.</typeparam>
	/// <seealso cref="IPipeConnector{T}" />
	public class ConditionalPipeConnector<T> : IPipeConnector<T>
	{
		private readonly Predicate<T> _predicate;
		private readonly IPipe<T> _truePipe;
		private readonly IPipe<T> _falsePipe;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalPipeConnector{T}"/> class.
		/// </summary>
		/// <param name="truePipe">The pipe used when the predicate returns true.</param>
		/// <param name="falsePipe">The pipe used when the predicate returns false.</param>
		/// <param name="predicate">The predicate used to determine to which pipe the data is given.</param>
		public ConditionalPipeConnector(IPipe<T> truePipe, IPipe<T> falsePipe, Predicate<T> predicate)
		{
			_falsePipe = falsePipe;
			_truePipe = truePipe;
			_predicate = predicate;
		}

		/// <inheritdoc />
		public void Process(IList<T> data)
		{
			var falsePipeData = new List<T>();
			var truePipeData = new List<T>();

			foreach (var item in data)
			{
				if (_predicate.Invoke(item))
				{
					truePipeData.Add(item);
				}
				else
				{
					falsePipeData.Add(item);
				}
			}

			if (truePipeData.Count > 0) _truePipe.Process(truePipeData);
			if (falsePipeData.Count > 0) _falsePipe.Process(falsePipeData);
		}
	}
}