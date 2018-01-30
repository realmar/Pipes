using System;

namespace SomePipes
{
    public class MathExponentiationProcessor : IPipeProcessor<double, double>
    {
        private double _exponent;

        public MathExponentiationProcessor(double exponent)
        {
            _exponent = exponent;
        }

        public double Process(double data)
        {
            return Math.Pow(data, _exponent);
        }
    }
}