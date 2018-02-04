using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Realmar.Pipes.Junctions;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.ProcessStrategies;
using Realmar.Pipes.Tests.SamplePipes.Misc;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Realmar.Pipes.Tests.SampleProcessors.String;
using Xunit;

namespace Realmar.Pipes.Tests.Integration
{
    public class PipeFixture
    {
        private class Base { }
        private class Derived : Base { }

        [Theory]
        [InlineData(1, 20)]
        [InlineData(2, 10)]
        public void Process_Single_Processor(double data, double multiplicator)
        {
            var pipe = new Pipe<double>();
            pipe.FirstConnector
                .Connect(new MultiplicationProcessor(multiplicator))
                .Finish(results =>
                {
                    Assert.Equal(1, results.Count);
                    Assert.Equal(results[0], data * multiplicator);
                });

            pipe.Process(data);
        }

        [Fact]
        public void Process_Multiple_Processors()
        {
            var startStr = "hello world";
            var str1 = "integration";
            var str2 = "test";

            var pipe = new Pipe<string>();
            pipe.FirstConnector
                .Connect(new AppendStringProcessor(str1))
                .Connect(new AppendStringProcessor(str2))
                .Connect(new ToUpperCaseProcessor())
                .Finish(results =>
                {
                    Assert.Equal(1, results.Count);
                    Assert.Equal(results[0], (startStr + str1 + str2).ToUpper());
                });

            pipe.Process(startStr);
        }

        [Fact]
        public void Process_ListData()
        {
            var data = new List<double> { 1d, 2d, 3d, 4d, 5d, 6d };
            var exponent = 2;

            var pipe = new Pipe<double>(new SerialProcessStrategy());
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
        public void Process_Parallel()
        {
            var waitTime = 500;

            var pipe = new Pipe<int>(new ParallelProcessStrategy());
            pipe.FirstConnector
                .Connect(new WaitProcessor<int>(waitTime))
                .Finish(results => { });

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            pipe.Process(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });

            stopwatch.Stop();
            Assert.True(stopwatch.Elapsed.TotalMilliseconds < waitTime * 1.1);
        }

        [Fact]
        public void Process_Parallel_Synchronize()
        {
            var waitTime = 500;
            var data = new List<int> { 1, 2, 3, 4 };

            var pipe1 = new Pipe<int>(new ParallelProcessStrategy());
            var pipe2 = new Pipe<int>(new ParallelProcessStrategy());

            var thisThreadId = Thread.CurrentThread.ManagedThreadId;
            var stopwatch = new Stopwatch();

            Action<IList<int>> assertionsAction = results =>
            {
                stopwatch.Stop();

                // Check if we are running in the same thread as we started
                Assert.Equal(thisThreadId, Thread.CurrentThread.ManagedThreadId);
                Assert.True(stopwatch.Elapsed.TotalMilliseconds < waitTime * 1.1);
                Assert.Equal(results.Count, data.Count);
                foreach (var d in data)
                {
                    Assert.True(results.Contains(d));
                }
            };

            pipe1.FirstConnector
                .Connect(new WaitProcessor<int>(waitTime))
                .Finish(results =>
                {
                    assertionsAction.Invoke(results);

                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    pipe2.Process(data);
                });

            pipe2.FirstConnector
                .Connect(new WaitProcessor<int>(waitTime))
                .Finish(results =>
                {
                    assertionsAction.Invoke(results);
                });

            stopwatch.Start();
            pipe1.Process(data);

        }

        [Fact]
        public void Process_ConditionalJunction()
        {
            var appendStr = " is your number!";

            var mathPipe = new Pipe<double>();
            var stringPipe = new Pipe<double>();

            var junction = new ConditionalJunction<double>(mathPipe, stringPipe, x => x < 20);

            mathPipe.FirstConnector
                .Connect(new AssertionProcessor<double>(x => Assert.True(x < 20)))
                .Connect(new MultiplicationProcessor(2))
                .Finish(junction.Process);

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
        public void Process_Variance()
        {
            var pipe = new Pipe<Derived>();
            pipe.FirstConnector
                .Connect(new DebugProcessor<Base, Base>(x => x))
                .Connect(new DebugProcessor<object, Derived>(x => x as Derived))
                .Connect(new DebugProcessor<Base, object>(x => x))
                .Finish(results =>
                {
                    Assert.Equal(1, results.Count);
                    Assert.IsType<Derived>(results[0]);
                });

            pipe.Process(new Derived());
        }
    }
}