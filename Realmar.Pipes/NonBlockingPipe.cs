using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Realmar.Pipes.ProcessStrategies;

namespace Realmar.Pipes
{
	/// <summary>
	/// The NonBlockingPipe class.
	/// Processes data without blocking the caller.
	/// </summary>
	/// <typeparam name="TIn">The type of the input data.</typeparam>
	/// <seealso cref="Realmar.Pipes.Pipe{TIn}" />
	/// <seealso cref="System.IDisposable" />
	public class NonBlockingPipe<TIn> : Pipe<TIn>, IDisposable
	{
		private volatile bool _stopProcessing;
		private Thread _workerThread;
		private readonly EventWaitHandle _waitHandle;

		private readonly List<TIn> _scheduledData;

		private readonly object _addResultLock;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonBlockingPipe{TIn}"/> class.
		/// </summary>
		/// <param name="strategy">The strategy used to process the data.</param>
		public NonBlockingPipe(IProcessStrategy strategy) : base(strategy)
		{
			_addResultLock = new object();
			_scheduledData = new List<TIn>();
			_waitHandle = new AutoResetEvent(false);
			_workerThread = new Thread(ThreadRunner);
			_workerThread.Start();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipe{TIn}"/> class
		/// using the <see cref="ThreadPoolProcessStrategy"/> strategy.
		/// </summary>
		public NonBlockingPipe() : this(new ThreadPoolProcessStrategy()) { }

		/// <summary>
		/// Finalizes an instance of the <see cref="NonBlockingPipe{TIn}"/> class.
		/// </summary>
#if DEBUG
		[ExcludeFromCodeCoverage]
#endif
		~NonBlockingPipe()
		{
			Dispose();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed) return;

			_stopProcessing = true;
			_waitHandle.Set();

			_waitHandle?.Dispose();
			_isDisposed = true;
		}

		/// <inheritdoc />
		/// <exception cref="ObjectDisposedException">The <see cref="M:System.Threading.WaitHandle.Close"></see> method was previously called on this <see cref="T:System.Threading.EventWaitHandle"></see>.</exception>
		public override void Process(IList<TIn> data)
		{
			lock (_addResultLock) _scheduledData.AddRange(data);
			_waitHandle.Set();
		}

		/// <inheritdoc />
		public override void AddResult(object result)
		{
			Callback.Invoke(new List<object> { result });
		}

		/// <summary>
		/// WorkerThread is running this method.
		/// </summary>
		private void ThreadRunner()
		{
			while (!_stopProcessing)
			{
				_waitHandle.WaitOne();

				if (_scheduledData.Count.Equals(0)) continue;
				lock (_addResultLock)
				{
					ProcessStrategy.Process(FirstConnector, _scheduledData);
					_scheduledData.Clear();
				}
			}
		}
	}
}