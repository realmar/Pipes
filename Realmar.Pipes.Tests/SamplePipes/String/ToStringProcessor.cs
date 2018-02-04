using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SampleProcessors.Processors.String
{
    public class ToStringProcessor<TIn> : IPipeProcessor<TIn, string>
    {
        public string Process(TIn data)
        {
            return data.ToString();
        }
    }
}