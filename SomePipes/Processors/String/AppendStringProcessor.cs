using System;

namespace SomePipes
{
    public class AppendStringProcessor : IPipeProcessor<string, string>
    {
        private Func<string> _stringAppendFunc;

        public AppendStringProcessor(Func<string> stringAppendFunc)
        {
            _stringAppendFunc = stringAppendFunc;
        }

        public string Process(string data)
        {
            return data + _stringAppendFunc.Invoke();
        }
    }
}