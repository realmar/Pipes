using System;
using SomePipes.Pipe;

namespace SomePipes.Junctions
{
    public class ConditionalPipeJunction<TIn> : IPipeJunction<TIn>
    {
        private readonly Predicate<TIn> _predicate;
        private readonly IPipe<TIn> _truePipe;
        private readonly IPipe<TIn> _falsePipe;

        public ConditionalPipeJunction(IPipe<TIn> falsePipe, IPipe<TIn> truePipe, Predicate<TIn> predicate)
        {
            _truePipe = truePipe;
            _falsePipe = falsePipe;
            _predicate = predicate;
        }

        public void Process(TIn data)
        {
            if (_predicate.Invoke(data))
            {
                _truePipe.Process(data);
            }
            else
            {
                _falsePipe.Process(data);
            }
        }
    }
}