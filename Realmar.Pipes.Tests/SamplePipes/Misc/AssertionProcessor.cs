using System;
using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SamplePipes.Misc
{
    public class AssertionProcessor<T> : IPipeProcessor<T, T>
    {
        private readonly Action<T> _assertionAction;

        public AssertionProcessor(Action<T> assertionAction)
        {
            _assertionAction = assertionAction;
        }

        public T Process(T data)
        {
            _assertionAction.Invoke(data);
            return data;
        }
    }
}