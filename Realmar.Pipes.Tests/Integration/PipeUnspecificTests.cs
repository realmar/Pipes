using System;
using System.Collections.Generic;
using System.Linq;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Realmar.Pipes.Tests.SampleProcessors.String;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
	public class PipeUnspecificTests
	{
		private class Base { }
		private class Derived : Base { }

		private IPipe<T> CreatePipe<T>(Type type)
		{
			return Activator.CreateInstance(type) as IPipe<T>;
		}

		private void DisposePipe(object pipe)
		{
			var pipeDisposable = pipe as IDisposable;
			pipeDisposable?.Dispose();
		}

		[Theory]
		[InlineData(typeof(Pipe<double>), 1, 20)]
		[InlineData(typeof(Pipe<double>), 2, 10)]
		[InlineData(typeof(NonBlockingPipe<double>), 1, 20)]
		[InlineData(typeof(NonBlockingPipe<double>), 2, 10)]
		public void Process_Single_Processor(Type pipeType, double data, double multiplicator)
		{
			var pipe = CreatePipe<double>(pipeType);
			pipe.FirstConnector
				.Connect(new MultiplicationProcessor(multiplicator))
				.Finish(results =>
				{
					Assert.Equal(1, results.Count);
					Assert.Equal(results[0], data * multiplicator);
				});

			pipe.Process(new List<double> { data });

			DisposePipe(pipe);
		}

		[Theory]
		[InlineData(typeof(Pipe<string>))]
		[InlineData(typeof(NonBlockingPipe<string>))]
		public void Process_Multiple_Processors(Type pipeType)
		{
			var pipe = CreatePipe<string>(pipeType);
			Assert.NotNull(pipe);

			var startStr = "hello world";
			var str1 = "integration";
			var str2 = "test";

			pipe.FirstConnector
				.Connect(new AppendStringProcessor(str1))
				.Connect(new AppendStringProcessor(str2))
				.Connect(new ToUpperCaseProcessor())
				.Finish(results =>
				{
					Assert.Equal(1, results.Count);
					Assert.Equal(results[0], (startStr + str1 + str2).ToUpper());
				});

			pipe.Process(new List<string> { startStr });

			DisposePipe(pipe);
		}

		[Theory]
		[InlineData(typeof(Pipe<double>))]
		[InlineData(typeof(NonBlockingPipe<double>))]
		public void Process_ListData(Type pipeType)
		{
			var pipe = CreatePipe<double>(pipeType);

			var data = new List<double> { 1d, 2d, 3d, 4d, 5d, 6d };
			var exponent = 2;

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

			DisposePipe(pipe);
		}

		[Theory]
		[InlineData(typeof(Pipe<double>))]
		[InlineData(typeof(NonBlockingPipe<double>))]
		public void Process_ConditionalPipeConnector(Type pipeType)
		{
			var appendStr = " is your number!";

			var mathPipe = CreatePipe<double>(pipeType);
			var stringPipe = CreatePipe<double>(pipeType);

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

			DisposePipe(mathPipe);
			DisposePipe(stringPipe);
		}

		[Theory]
		[InlineData(typeof(Pipe<Derived>))]
		[InlineData(typeof(NonBlockingPipe<Derived>))]
		public void Process_Variance(Type pipeType)
		{
			var pipe = CreatePipe<Derived>(pipeType);
			pipe.FirstConnector
				.Connect(new DebugProcessor<Base, Base>(x => x))
				.Connect(new DebugProcessor<object, Derived>(x => x as Derived))
				.Connect(new DebugProcessor<Base, object>(x => x))
				.Finish(results =>
				{
					Assert.Equal(1, results.Count);
					Assert.IsType<Derived>(results[0]);
				});

			pipe.Process(new List<Derived> { new Derived() });

			DisposePipe(pipe);
		}
	}
}