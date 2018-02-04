using System;

namespace Realmar.Pipes.Processors.Misc
{
    public class CastProcessor<T> : IPipeProcessor<object, T>
    {
        public T Process(object data)
        {
            // check if data is boxed, if yes then we cannot cast it directly
            if (typeof(T).IsValueType)
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }

            return (T)data;
        }
    }
}