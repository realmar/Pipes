﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.ProcessStrategies;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Realmar.Pipes.Tests.SampleProcessors.String;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
	public class PipeTests
	{
		[Fact]
		public void Process_ListData()
		{
			var data = new List<double> { 1d, 2d, 3d, 4d, 5d, 6d };
			var exponent = 2;

			var pipe = new Pipe<double>();
			pipe.FirstConnector
				.Connect(new ExponentiationProcessor(exponent))
				.Connect(new ExponentiationProcessor(exponent))
				.Finish(results =>
				{
					Assert.Equal(data.Count, results.Count);

					// Note: this only works because we are executing in serial
					// otherwise the order of results will not be the same as
					// in data
					foreach (var i in Enumerable.Range(0, data.Count))
					{
						Assert.Equal(results[i], Math.Pow(data[i], exponent * exponent));
					}
				});

			pipe.Process(data);
		}

		[Fact]
		public void Process_ConditionalPipeConnector()
		{
			var appendStr = " is your number!";

			var mathPipe = new Pipe<double>();
			var stringPipe = new Pipe<double>();

			var connector = new ConditionalPipeConnector<double>(mathPipe, stringPipe, x => x < 20);

			mathPipe.FirstConnector
				.Connect(new AssertionProcessor<double>(x => Assert.True(x < 20)))
				.Connect(new MultiplicationProcessor(2))
				.Finish(connector.Process);

			stringPipe.FirstConnector
				.Connect(new AssertionProcessor<double>(x => Assert.True(x >= 20)))
				.Connect(new ToStringProcessor<double>())
				.Connect(new AppendStringProcessor(appendStr))
				.Finish(results =>
				{
					foreach (var result in results)
					{
						Assert.EndsWith(appendStr, result);
					}
				});

			mathPipe.Process(new List<double> { 1, 2, 3, 4, 5, 6 });
		}

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