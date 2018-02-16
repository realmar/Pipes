﻿using System;
using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.ProcessStrategies;

namespace Realmar.Pipes
{
	/// <inheritdoc />
	public class Pipe<TIn> : IPipe<TIn>
	{
		public IProcessorConnector<TIn> FirstConnector { get; }
		public Action<IList<object>> Callback { protected get; set; }

		private readonly IProcessStrategy _processStrategy;
		private IList<object> _results;
		private readonly object _lock;

		protected IProcessStrategy ProcessStrategy => _processStrategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipe{TIn}"/> class.
		/// </summary>
		/// <param name="strategy">The strategy used to process the data.</param>
		public Pipe(IProcessStrategy strategy)
		{
			_processStrategy = strategy;
			FirstConnector = new ProcessorConnector<TIn>(this);

			_lock = new Mutex();
			_results = new List<object>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipe{TIn}"/> class
		/// using the <see cref="SerialProcessStrategy"/> strategy.
		/// </summary>
		public Pipe() : this(new SerialProcessStrategy()) { }

		/// <inheritdoc />
		public virtual void Process(IList<TIn> data)
		{
			_processStrategy.Process(FirstConnector, data);

			var results = _results;
			_results = new List<object>();
			Callback.Invoke(results);
		}

		/// <summary>
		/// <see cref="IPipe{TIn}.Process"/> overload which takes a single item as input instead of a list.
		/// </summary>
		/// <param name="data">The data.</param>
		public void Process(TIn data)
		{
			Process(new[] { data });
		}

		/// <inheritdoc />
		public virtual void AddResult(object result)
		{
			lock (_lock)
			{
				_results.Add(result);
			}
		}
	}
}