namespace SomePipes.Junctions
{
    public interface IPipeJunction<in TIn>
    {
        void Process(TIn data);
    }
}