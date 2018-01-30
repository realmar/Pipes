namespace SomePipes
{
    public class Pipe<TIn> : IPipe<TIn>
    {
        public IProcessStrategy ProcessStrategy { get; set; }

        public IPipeConnector<TIn> FirstConnector { get; }

        public Pipe()
        {
            ProcessStrategy = new SerialProcessStrategy();
            FirstConnector = new PipeConnector<TIn>();
        }

        public void Process(TIn data)
        {
            ProcessStrategy.Process(FirstConnector, data);
        }

        public void Process(TIn[] data)
        {
            ProcessStrategy.Process(FirstConnector, data);
        }
    }
}