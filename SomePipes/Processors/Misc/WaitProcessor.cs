using System.Threading;

namespace SomePipes.Processors.Misc
{
    public class WaitProcessor<T> : IPipeProcessor<T, T>
    {
        private readonly int _waitTime;

        public WaitProcessor(int waitTime)
        {
            _waitTime = waitTime;
        }

        public T Process(T data)
        {
            Thread.Sleep(_waitTime);
            return data;
        }
    }
}