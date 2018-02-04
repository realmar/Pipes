using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SampleProcessors.String
{
    public class ToUpperCaseProcessor : IPipeProcessor<string, string>
    {
        public string Process(string data)
        {
            return data.ToUpperInvariant();
        }
    }
}