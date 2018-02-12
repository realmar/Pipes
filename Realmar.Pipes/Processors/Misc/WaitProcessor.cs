using System.Threading;

namespace Realmar.Pipes.Processors.Misc
{
	/// <summary>
	/// The WaitProcessor class.
	/// Takes in some data, waits for a specified amount of time and returns the data without
	/// performing any modification on it.
	/// </summary>
	/// <typeparam name="T">The input data.</typeparam>
	/// <seealso cref="Realmar.Pipes.Processors.IPipeProcessor{T, T}" />
	public class WaitProcessor<T> : IPipeProcessor<T, T>
	{
		private readonly int _waitTime;

		/// <summary>
		/// Initializes a new instance of the <see cref="WaitProcessor{T}"/> class.
		/// </summary>
		/// <param name="waitTime">The time to wait in milliseconds.</param>
		public WaitProcessor(int waitTime)
		{
			_waitTime = waitTime;
		}

		/// <inheritdoc />
		public T Process(T data)
		{
			Thread.Sleep(_waitTime);
			return data;
		}
	}
}