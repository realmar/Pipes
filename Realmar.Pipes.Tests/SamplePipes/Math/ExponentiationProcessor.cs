using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SampleProcessors.Processors.Math
{
    public class ExponentiationProcessor : IPipeProcessor<double, double>
    {
        private double _exponent;

        public ExponentiationProcessor(double exponent)
        {
            _exponent = exponent;
        }

        public double Process(double data)
        {
            return System.Math.Pow(data, _exponent);
        }
    }
}