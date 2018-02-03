using System.Collections.Generic;
using SomePipes.Pipe;

namespace SomePipes.ProcessStrategies
{
    public interface IProcessStrategy
    {
        void Process<TIn>(IPipe<TIn> pipe, IList<TIn> data);
    }
}