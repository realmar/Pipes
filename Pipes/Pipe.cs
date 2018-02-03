﻿using System;
using System.Collections.Generic;
using System.Threading;
using Realmar.Pipes.Connector;
using Realmar.Pipes.ProcessStrategies;

namespace Realmar.Pipes
{
    public class Pipe<TIn> : IPipe<TIn>
    {
        public IProcessStrategy ProcessStrategy { get; set; }
        public IPipeConnector<TIn> FirstConnector { get; }
        public Action<IList<object>> Callback { private get; set; }

        private IList<object> _results;
        private readonly Mutex _mutex;

        public Pipe()
        {
            ProcessStrategy = new SerialProcessStrategy();
            FirstConnector = new PipeConnector<TIn>(this);

            _mutex = new Mutex();
            _results = new List<object>();
        }

        public void Process(IList<TIn> data)
        {
            ProcessStrategy.Process(this, data);

            var results = _results;
            _results = new List<object>();
            Callback.Invoke(results);
        }

        public void Process(TIn data)
        {
            Process(new[] { data });
        }

        public void AddResult(object result)
        {
            lock (_mutex)
            {
                _results.Add(result);
            }
        }
    }
}