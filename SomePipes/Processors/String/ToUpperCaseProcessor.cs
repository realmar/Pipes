namespace SomePipes
{
    public class ToUpperCaseProcessor : IPipeProcessor<string, string>
    {
        public string Process(string data)
        {
            return data.ToUpperInvariant();
        }
    }
}