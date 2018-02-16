using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.ProcessStrategies;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
	public class PipeTests
	{
		[Fact]
		public void Process_Parallel()
		{
			var waitTime = 500;
			var data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

			var pipe = new Pipe<int>(new ParallelProcessStrategy());
			pipe.FirstConnector
				.Connect(new WaitProcessor<int>(waitTime))
				.Finish(results => { });

			var stopwatch = new Stopwatch();

			stopwatch.Start();
			pipe.Process(data);
			stopwatch.Stop();

			Assert.True(stopwatch.Elapsed.TotalMilliseconds < waitTime * data.Count / 2);
		}

		[Fact]
		public void Process_Parallel_Synchronize()
		{
			var startingThreadId = Thread.CurrentThread.ManagedThreadId;
			var waitTime = 500;
			var data = new List<int> { 1, 2, 3, 4 };

			var pipe1 = new Pipe<int>(new ParallelProcessStrategy());

			pipe1.FirstConnector
				.Connect(new WaitProcessor<int>(waitTime))
				.Finish(results =>
				{
					Assert.Equal(startingThreadId, Thread.CurrentThread.ManagedThreadId);
					Assert.Equal(data.Count, results.Count);
				});

			pipe1.Process(data);
		}
	}
}