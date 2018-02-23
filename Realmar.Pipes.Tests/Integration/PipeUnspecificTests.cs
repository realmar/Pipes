using System;
using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Realmar.Pipes.Tests.SampleProcessors.String;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
	public class PipeUnspecificTests
	{
		private class Base
		{
		}

		private class Derived : Base
		{
		}

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
			using (var waitHandle = new AutoResetEvent(false))
			{
				var pipe = CreatePipe<double>(pipeType);
				pipe.FirstConnector
					.Connect(new MultiplicationProcessor(multiplicator))
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						Assert.Equal(results[0], data * multiplicator);

						waitHandle.Set();
					});

				pipe.Process(new List<double> { data });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}

		[Theory]
		[InlineData(typeof(Pipe<string>))]
		[InlineData(typeof(NonBlockingPipe<string>))]
		public void Process_Multiple_Processors(Type pipeType)
		{
			using (var waitHandle = new AutoResetEvent(false))
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

						waitHandle.Set();
					});

				pipe.Process(new List<string> { startStr });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}

		[Theory]
		[InlineData(typeof(Pipe<Derived>))]
		[InlineData(typeof(NonBlockingPipe<Derived>))]
		public void Process_Variance(Type pipeType)
		{
			using (var waitHandle = new AutoResetEvent(false))
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

						waitHandle.Set();
					});

				pipe.Process(new List<Derived> { new Derived() });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}

		[Theory]
		[InlineData(typeof(Pipe<double>))]
		[InlineData(typeof(NonBlockingPipe<double>))]
		public void Process_Single_Delegate(Type pipeType)
		{
			using (var waitHandle = new AutoResetEvent(false))
			{
				var data = 8;
				var mutliplicator = 2;

				var pipe = CreatePipe<double>(pipeType);

				pipe.FirstConnector
					.Connect(x => x * mutliplicator)
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						Assert.Equal(results[0], data * mutliplicator);

						waitHandle.Set();
					});

				pipe.Process(new List<double> { data });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}

		[Theory]
		[InlineData(typeof(Pipe<string>))]
		[InlineData(typeof(NonBlockingPipe<string>))]
		public void Process_Multiple_Delegates(Type pipeType)
		{
			using (var waitHandle = new AutoResetEvent(false))
			{
				var data = "Hello Mars";
				const string dataUpperCase = "HELLO MARS";

				var pipe = CreatePipe<string>(pipeType);
				pipe.FirstConnector
					.Connect(str => str.ToUpperInvariant())
					.Connect(str =>
					{
						Assert.Equal(dataUpperCase, str);
						return str;
					})
					.Connect(str => str.IndexOf('A'))
					.Connect(index =>
					{
						Assert.Equal(7, index);
						return index;
					})
					.Connect(index => index * 2)
					.Connect(index => index - 2)
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						Assert.Equal(results[0], 7 * 2 - 2);

						waitHandle.Set();
					});

				pipe.Process(new List<string> { data });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}

		[Theory]
		[InlineData(typeof(Pipe<Derived>))]
		[InlineData(typeof(NonBlockingPipe<Derived>))]
		public void Process_Variance_Delegates(Type pipeType)
		{
			using (var waitHandle = new AutoResetEvent(false))
			{
				Func<Base, Base> baseToBase = x => x;
				Func<object, Derived> objectToDerived = x => x as Derived;
				Func<Base, object> baseToObject = x => x;

				var pipe = CreatePipe<Derived>(pipeType);
				pipe.FirstConnector
					.Connect(baseToBase)
					.Connect(objectToDerived)
					.Connect(baseToObject)
					.Finish(results =>
					{
						Assert.Equal(1, results.Count);
						Assert.IsType<Derived>(results[0]);

						waitHandle.Set();
					});

				pipe.Process(new List<Derived> { new Derived() });

				waitHandle.WaitOne();
				DisposePipe(pipe);
			}
		}
	}
}