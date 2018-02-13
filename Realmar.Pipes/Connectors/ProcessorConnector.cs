using System;
using System.Collections.Generic;
using System.Linq;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Connectors
{
	/// <inheritdoc />
	public class ProcessorConnector<TIn> : IProcessorConnector<TIn>
	{
		private bool _isFinalProcessor;
		private Action<TIn> _processDelegate;

		private readonly IPipeResultReceiver _resultReceiver;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorConnector{TIn}"/> class.
		/// </summary>
		/// <param name="resultReceiver">The receiver of the processed data.</param>
		public ProcessorConnector(IPipeResultReceiver resultReceiver)
		{
			_resultReceiver = resultReceiver;
		}

		/// <inheritdoc />
		public IProcessorConnector<TOut> Connect<TOut>(IPipeProcessor<TIn, TOut> processor)
		{
			var connector = new ProcessorConnector<TOut>(_resultReceiver);
			_processDelegate = data => connector.Process(processor.Process(data));

			return connector;
		}

		/// <inheritdoc />
		public void Finish(Action<IList<TIn>> callback)
		{
			_isFinalProcessor = true;
			_resultReceiver.Callback = objects =>
			{
				var typedList = new List<TIn>(objects.Cast<TIn>());
				callback(typedList);
			};
		}

		/// <inheritdoc />
		public void Process(TIn data)
		{
			if (_isFinalProcessor)
			{
				_resultReceiver.AddResult(data);
			}
			else
			{
				_processDelegate?.Invoke(data);
			}
		}
	}
}