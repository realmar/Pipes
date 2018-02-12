namespace Realmar.Pipes.Processors
{
	/// <summary>
	/// The IPipeProcessor interface.
	/// Processes the input data and returns it.
	/// </summary>
	/// <typeparam name="TIn">The type of the input data.</typeparam>
	/// <typeparam name="TOut">The type of the output data.</typeparam>
	public interface IPipeProcessor<in TIn, out TOut>
	{
		/// <summary>
		/// Processes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>The processed data as TOut</returns>
		TOut Process(TIn data);
	}
}