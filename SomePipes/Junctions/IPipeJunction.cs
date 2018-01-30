namespace SomePipes
{
    public interface IPipeJunction<in TIn>
    {
        void Process(TIn data);
    }
}