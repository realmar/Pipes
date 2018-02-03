namespace Realmar.Pipes.Processors
{
    public interface IPipeProcessor<in TIn, out TOut>
    {
        TOut Process(TIn data);
    }
}