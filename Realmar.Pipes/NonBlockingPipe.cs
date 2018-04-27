using System;
using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.ProcessStrategies;

#if DEBUG
using System.Diagnostics.CodeAnalysis;
#endif

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
		private readonly Thread _workerThread;
		private readonly EventWaitHandle _waitHandle;

		private List<TIn> _scheduledData;
		private List<object> _processedData;

		private readonly object _scheduledDataLock;
		private readonly object _processedDataLock;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonBlockingPipe{TIn}"/> class.
		/// </summary>
		/// <param name="strategy">The strategy used to process the data.</param>
		/// <exception cref="ThreadStateException">The thread has already been started.</exception>
		/// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
		public NonBlockingPipe(IProcessStrategy strategy) : base(strategy)
		{
			_scheduledDataLock = new object();
			_processedDataLock = new object();

			_scheduledData = new List<TIn>();
			_processedData = new List<object>();

			_waitHandle = new AutoResetEvent(false);

			_workerThread = new Thread(ThreadRunner);
			_workerThread.Start();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipe{TIn}"/> class
		/// using the <see cref="ThreadPoolProcessStrategy"/> strategy.
		/// </summary>
		/// <exception cref="ThreadStateException">The thread has already been started.</exception>
		/// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
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
			if (_isDisposed) throw new ObjectDisposedException("Object has been disposed.");

			lock (_scheduledDataLock) _scheduledData.AddRange(data);
			_waitHandle.Set();
		}

		/// <inheritdoc />
		/// <exception cref="ObjectDisposedException">Object has been disposed.</exception>
		public override void AddResult(object result)
		{
			if (_isDisposed) throw new ObjectDisposedException("Object has been disposed.");

			lock (_processedDataLock) _processedData.Add(result);
			_waitHandle.Set();
		}

		/// <summary>
		/// WorkerThread is running this method.
		/// </summary>
		private void ThreadRunner()
		{
			while (!_stopProcessing)
			{
				_waitHandle.WaitOne();

				if (!0.Equals(_processedData.Count))
				{
					lock (_processedDataLock)
					{
						var data = _processedData;
						ThreadPool.QueueUserWorkItem(obj => Callback.Invoke(data));
						_processedData = new List<object>();
					}
				}

				if (!0.Equals(_scheduledData.Count))
				{
					lock (_scheduledDataLock)
					{
						ProcessStrategy.Process(FirstConnector, _scheduledData);
						_scheduledData = new List<TIn>();
					}
				}
			}
		}
	}
}
