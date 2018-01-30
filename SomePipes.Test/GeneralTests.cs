using Xunit;
using Xunit.Abstractions;

namespace SomePipes.Test
{
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

            var conditionalJunction = new ConditionalPipeJunction<double>(mathPipe, toStringPipe, x => x > 20);

            mathPipe.FirstConnector
                .Connect(new MathExponentiationProcessor(2))
                .Connect(new MathMultiplicationPipe(10))
                .Finish(conditionalJunction.Process);

            toStringPipe.FirstConnector
                .Connect(new ToStringProcessor<double>())
                .Connect(new AppendStringProcessor(() => " is your result"))
                .Connect(new ToUpperCaseProcessor())
                .Finish(result => _output.WriteLine(result));


            mathPipe.Process(1.1);

            Assert.True(true);
        }
    }
}