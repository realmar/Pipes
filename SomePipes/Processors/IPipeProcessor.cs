namespace SomePipes
{
    public interface IPipeProcessor<in TIn, out TOut>
    {
        TOut Process(TIn data);
    }
}