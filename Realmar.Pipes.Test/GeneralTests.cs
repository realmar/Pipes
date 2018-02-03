using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Realmar.Pipes.Junctions;
using Realmar.Pipes.Processors.Math;
using Realmar.Pipes.Processors.Misc;
using Realmar.Pipes.Processors.String;
using Realmar.Pipes.ProcessStrategies;
using Xunit;
using Xunit.Abstractions;

namespace Realmar.Pipes.Test
{
    internal static class ListExtensions
    {
        internal static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }

    public class GeneralTests
    {
        private readonly ITestOutputHelper _output;

        public GeneralTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        /// <summary>
        /// Not a real test (asserts true) but a playground. (For now)
        /// </summary>
        [Fact]
        public void Test1()
        {
            var mathPipe = new Pipe<double>();
            var toStringPipe = new Pipe<double>();

            var conditionalJunction = new ConditionalJunction<double>(mathPipe, toStringPipe, x => x > 20);

            mathPipe.FirstConnector
                .Connect(new ExponentiationProcessor(2))
                .Connect(new MultiplicationProcessor(10))
                .Finish(conditionalJunction.Process);

            toStringPipe.FirstConnector
                .Connect(new ToStringProcessor<double>())
                .Connect(new AppendStringProcessor(() => " is your result"))
                .Connect(new ToUpperCaseProcessor())
                .Finish(results => results.ForEach(result => _output.WriteLine(result)));

            mathPipe.Process(1.1);

            Assert.True(true);
        }

        [Fact]
        public void ParallelTest()
        {
            var calcPipe = new Pipe<double>
            {
                ProcessStrategy = new ParallelProcessStrategy()
            };

            var presentationPipe = new Pipe<double>
            {
                ProcessStrategy = new ParallelProcessStrategy()
            };

            var junction = new ConditionalJunction<double>(calcPipe, presentationPipe, x => x > 200);

            var stopwatch = new Stopwatch();

            calcPipe.FirstConnector
                .Connect(new WaitProcessor<double>(500))
                .Connect(new MultiplicationProcessor(10))
                .Finish(junction.Process);

            presentationPipe.FirstConnector
                .Connect(new ToStringProcessor<double>())
                .Connect(new AppendStringProcessor(() => " here you go"))
                .Connect(new ToUpperCaseProcessor())
                .Finish(results => results.ForEach(result => _output.WriteLine(result.ToString())));

            var data = Enumerable.Range(1, 10)
                .Select(x => (double)x)
                .ToArray();

            stopwatch.Start();
            calcPipe.Process(data);
            stopwatch.Stop();

            _output.WriteLine("Elapsed Time Parallel: " + stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}