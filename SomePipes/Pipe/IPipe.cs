namespace SomePipes
{
    public interface IPipe<TIn>
    {
        IProcessStrategy ProcessStrategy { set; }
        IPipeConnector<TIn> FirstConnector { get; }

        void Process(TIn data);
    }
}