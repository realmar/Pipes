using System;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SamplePipes.Misc
{
    public class DebugProcessor<TIn, TOut> : IPipeProcessor<TIn, TOut>
    {
        private Func<TIn, TOut> _debugFunc;

        public DebugProcessor(Func<TIn, TOut> debugFunc)
        {
            _debugFunc = debugFunc;
        }

        public TOut Process(TIn data)
        {
            return _debugFunc.Invoke(data);
        }
    }
}