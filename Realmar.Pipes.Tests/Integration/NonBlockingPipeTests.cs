using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Realmar.Pipes.Tests.SampleProcessors.String;
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

				// Assert that waitHandle has not been set yet
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

				/* I'm not sure how to do this better?
				 * I need to wait for the pipe to process all data, however I cannot
				 * be sure that the pipe even processes the data. (or processes the
				 * data correctly) Therefore I cannot just use a WaitHandle and wait until
				 * processedData contains 6 elements, as this may never happen.
				 *
				 * The problem is that when Thread.Sleep is given an in which is too small (eg. 500ms)
				 * then the appveyor and travis CI will fail as the pipe has not finished the data and will
				 * be Disposed while still needing to process data.
				 * This does not seem to happen on my local machine as it is faster than then CI.
				 */
				Thread.Sleep(2000);

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

				// Same here, I'm not sure how to do this better
				Thread.Sleep(2000);

				Assert.Equal(8, processedData.Count);
				for (var i = 0; i < 8; i++)
					Assert.Contains(i * 2 * 2, processedData);
			}
		}

		[Fact]
		public void Process_ConditionalPipeConnector()
		{
			using (var mathPipe = new NonBlockingPipe<double>())
			using (var stringPipe = new NonBlockingPipe<double>())
			{
				var appendStr = " is your number!";

				var connector = new ConditionalPipeConnector<double>(mathPipe, stringPipe, x => x < 20);

				mathPipe.FirstConnector
					.Connect(new AssertionProcessor<double>(x => Assert.True(x < 20)))
					.Connect(new MultiplicationProcessor(2))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						connector.Process(results);
					});

				stringPipe.FirstConnector
					.Connect(new AssertionProcessor<double>(x => Assert.True(x >= 20)))
					.Connect(new ToStringProcessor<double>())
					.Connect(new AppendStringProcessor(appendStr))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						Assert.EndsWith(appendStr, results[0]);
					});

				// Same issue as above
				Thread.Sleep(2000);

				mathPipe.Process(new List<double> { 1, 2, 3, 4, 5, 6 });
			}
		}
	}
}