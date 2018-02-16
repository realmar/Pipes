using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
	public class NonBlockingPipeTests
	{
		[Fact]
		public void Process_DoesNotBlock()
		{
			using (var waitHandle = new AutoResetEvent(false))
			using (var pipe = new NonBlockingPipe<string>())
			{
				pipe.FirstConnector
					.Connect(new WaitProcessor<string>(500))
					.Finish(x => waitHandle.Set());
				pipe.Process(new List<string> { "A", "B" });

				Assert.False(waitHandle.WaitOne(0));
				waitHandle.WaitOne();
			}
		}

		[Fact]
		public void Process_GiveDataContinuously()
		{
			using (var pipe = new NonBlockingPipe<int>())
			{
				var processedData = new List<int>();
				var @lock = new object();

				pipe.FirstConnector
					.Connect(new DebugProcessor<int, int>(x => x))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						lock (@lock) processedData.AddRange(results);
					});

				pipe.Process(0);
				pipe.Process(1);
				pipe.Process(new List<int> { 2, 3 });
				pipe.Process(new List<int> { 4, 5 });

				Thread.Sleep(500);

				Assert.Equal(6, processedData.Count);

				for (var i = 0; i < 6; i++)
					Assert.Contains(i, processedData);
			}
		}

		[Fact]
		public void Process_GiveDataContinuously_MultiplePipes()
		{
			using (var pipe1 = new NonBlockingPipe<double>())
			using (var pipe2 = new NonBlockingPipe<double>())
			{
				var processedData = new List<double>();
				var @lock = new object();

				pipe1.FirstConnector
					.Connect(new MultiplicationProcessor(2))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						pipe2.Process(results);
					});

				pipe2.FirstConnector
					.Connect(new MultiplicationProcessor(2))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						lock (@lock) processedData.AddRange(results);
					});

				pipe1.Process(new List<double> { 0, 1, 2, 3 });
				pipe1.Process(new List<double> { 4, 5, 6, 7 });

				Thread.Sleep(1000);

				Assert.Equal(8, processedData.Count);
				for (var i = 0; i < 8; i++)
					Assert.Contains(i * 2 * 2, processedData);
			}
		}
	}
}