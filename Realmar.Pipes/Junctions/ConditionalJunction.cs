using System;
using System.Collections.Generic;

namespace Realmar.Pipes.Junctions
{
    public class ConditionalJunction<TIn> : IPipeJunction<TIn>
    {
        private readonly Predicate<TIn> _predicate;
        private readonly IPipe<TIn> _truePipe;
        private readonly IPipe<TIn> _falsePipe;

        public ConditionalJunction(IPipe<TIn> truePipe, IPipe<TIn> falsePipe, Predicate<TIn> predicate)
        {
            _falsePipe = falsePipe;
            _truePipe = truePipe;
            _predicate = predicate;
        }

        public void Process(IList<TIn> data)
        {
            var falsePipeData = new List<TIn>();
            var truePipeData = new List<TIn>();

            foreach (var item in data)
            {
                if (_predicate.Invoke(item))
                {
                    truePipeData.Add(item);
                }
                else
                {
                    falsePipeData.Add(item);
                }
            }

            if (truePipeData.Count > 0) _truePipe.Process(truePipeData);
            if (falsePipeData.Count > 0) _falsePipe.Process(falsePipeData);
        }
    }
}