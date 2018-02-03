using System;
using System.Collections.Generic;
using System.Linq;
using SomePipes.Pipe;
using SomePipes.Processors;

namespace SomePipes.Connector
{
    public class PipeConnector<TIn> : IPipeConnector<TIn>
    {
        private bool _isFinalProcessor;
        private Action<TIn> _processDelegate;

        private readonly IPipeResultReceiver _resultReceiver;

        public PipeConnector(IPipeResultReceiver resultReceiver)
        {
            _resultReceiver = resultReceiver;
        }

        public IPipeConnector<TOut> Connect<TOut>(IPipeProcessor<TIn, TOut> processor)
        {
            var connector = new PipeConnector<TOut>(_resultReceiver);
            _processDelegate = data => connector.Process(processor.Process(data));

            return connector;
        }

        public void Finish(Action<IList<TIn>> callback)
        {
            _isFinalProcessor = true;
            _resultReceiver.Callback = objects =>
            {
                var typedList = new List<TIn>(objects.Cast<TIn>());
                callback(typedList);
            };
        }

        public void Process(TIn data)
        {
            if (_isFinalProcessor)
            {
                _resultReceiver.AddResult(data);
            }
            else
            {
                _processDelegate?.Invoke(data);
            }
        }
    }
}