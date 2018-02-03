using System.Collections.Generic;

namespace Realmar.Pipes.ProcessStrategies
{
    public interface IProcessStrategy
    {
        void Process<TIn>(IPipe<TIn> pipe, IList<TIn> data);
    }
}