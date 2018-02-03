using System.Collections.Generic;

namespace Realmar.Pipes.Junctions
{
    public interface IPipeJunction<TIn>
    {
        void Process(IList<TIn> data);
    }
}