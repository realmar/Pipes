namespace Realmar.Pipes.Processors.Math
{
    public class MultiplicationPipe : IPipeProcessor<double, double>
    {
        private readonly double _multiplicator;

        public MultiplicationPipe(double multiplicator)
        {
            _multiplicator = multiplicator;
        }

        public double Process(double data)
        {
            return data * _multiplicator;
        }
    }
}