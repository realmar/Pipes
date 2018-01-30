namespace SomePipes
{
    public class MathMultiplicationPipe : IPipeProcessor<double, double>
    {
        private double _multiplicator;

        public MathMultiplicationPipe(double multiplicator)
        {
            _multiplicator = multiplicator;
        }

        public double Process(double data)
        {
            return data * _multiplicator;
        }
    }
}