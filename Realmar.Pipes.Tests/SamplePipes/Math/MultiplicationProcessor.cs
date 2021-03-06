﻿using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SampleProcessors.Math
{
    public class MultiplicationProcessor : IPipeProcessor<double, double>
    {
        private readonly double _multiplicator;

        public MultiplicationProcessor(double multiplicator)
        {
            _multiplicator = multiplicator;
        }

        public double Process(double data)
        {
            return data * _multiplicator;
        }
    }
}