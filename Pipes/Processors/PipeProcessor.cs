﻿namespace Realmar.Pipes.Processors
{
    public class PipeProcessor<TIn, TOut> : IPipeProcessor<TIn, TOut>
    {
        public TOut Process(TIn data)
        {
            return default;
        }
    }
}