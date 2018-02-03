namespace SomePipes.Processors.Misc
{
    public class CastProcessor<T> : IPipeProcessor<object, T>
    {
        public T Process(object data)
        {
            return (T)data;
        }
    }
}