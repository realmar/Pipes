using Realmar.Pipes.Processors;

namespace Realmar.Pipes.Tests.SampleProcessors.String
{
    public class AppendStringProcessor : IPipeProcessor<string, string>
    {
        private string _toAppend;

        public AppendStringProcessor(string toAppend)
        {
            _toAppend = toAppend;
        }

        public string Process(string data)
        {
            return data + _toAppend;
        }
    }
}