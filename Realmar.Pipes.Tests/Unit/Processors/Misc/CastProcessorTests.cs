using Realmar.Pipes.Processors.Misc;
using Xunit;

namespace Realmar.Pipes.Tests.Unit.Processors.Misc
{
    public class CastProcessorTests
    {
        private struct ExampleStruct { }
        private class ExampleClass { }

        private interface IExample { }
        private class ExampleConcrete : IExample { }

        [Fact]
        public void Process_ValueType()
        {
            var processor = new CastProcessor<ExampleStruct>();
            var data = new ExampleStruct() as object;

            // implicit "DoesNotThrow" assertion
            var result = processor.Process(data);
        }

        [Fact]
        public void Process_ReferenceType()
        {
            var processor = new CastProcessor<ExampleClass>();
            var data = new ExampleClass() as object;

            var result = processor.Process(data);
            Assert.IsType<ExampleClass>(result);
        }

        [Fact]
        public void Process_DoubleToInt()
        {
            var processor = new CastProcessor<int>();
            var result = processor.Process(2.78934d);

            Assert.Equal(3, result);
        }

        [Fact]
        public void Process_InterfaceToConcrete()
        {
            var processor = new CastProcessor<ExampleConcrete>();
            IExample data = new ExampleConcrete();
            var result = processor.Process(data);

            Assert.IsType<ExampleConcrete>(result);
        }

        [Fact]
        public void Process_ConcreteToInterface()
        {
            var processor = new CastProcessor<IExample>();
            var data = new ExampleConcrete();
            var result = processor.Process(data);

            Assert.IsAssignableFrom<IExample>(result);
        }
    }
}