using System;
using System.Reflection;
using System.Threading;
using Xunit;

namespace Realmar.Pipes.Tests.Unit.Pipes
{
	public class NonBlockingPipeTests : IDisposable
	{
		private NonBlockingPipe<object> _pipe;

		private Thread PipeThread => _pipe
			.GetType()
			.GetField("_workerThread", BindingFlags.Instance | BindingFlags.NonPublic)
			.GetValue(_pipe) as Thread;

		public NonBlockingPipeTests()
		{
			_pipe = new NonBlockingPipe<object>();
		}

		public void Dispose()
		{
			_pipe.Dispose();
		}

		[Fact]
		public void Dispose_DoNotThrowWhenCalledTwice()
		{
			_pipe.Dispose();

			// implicit does not throw assertion
			_pipe.Dispose();
		}

		[Fact]
		public void Dispose_ThreadCreated()
		{
			Assert.Equal(ThreadState.WaitSleepJoin, PipeThread.ThreadState);
		}

		[Fact]
		public void Dispose_ThreadExited()
		{
			_pipe.Dispose();

			// give thread time to terminate
			Thread.Sleep(500);
			Assert.Equal(ThreadState.Stopped, PipeThread.ThreadState);
		}
	}
}