using System;
using SomePipes.Processors;

namespace SomePipes.Connector
{
    public class PipeConnector<TIn> : IPipeConnector<TIn>
    {
        private delegate void ProcessDelegate(TIn data);

        private bool _isFinalProcessor;
        private Action<TIn> _callback;
        private ProcessDelegate _processDelegate;

        public IPipeConnector<TOut> Connect<TOut>(IPipeProcessor<TIn, TOut> processor)
        {
            var connector = new PipeConnector<TOut>();
            _processDelegate = data => connector.Process(processor.Process(data));

            return connector;
        }

        public void Finish(Action<TIn> callback)
        {
            _isFinalProcessor = true;
            _callback = callback;
        }

        public void Process(TIn data)
        {
            if (_isFinalProcessor)
            {
                _callback?.Invoke(data);
            }
            else
            {
                _processDelegate?.Invoke(data);
            }
        }
    }
}