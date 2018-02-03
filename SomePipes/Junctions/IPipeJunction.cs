using System.Collections.Generic;

namespace SomePipes.Junctions
{
    public interface IPipeJunction<TIn>
    {
        void Process(IList<TIn> data);
    }
}