using System.Collections.Generic;

namespace Realmar.Pipes.Connectors
{
	/// <summary>
	/// The IPipeConnector interface.
	/// Connects two or more pipes with each other by taking the output of
	/// one pipe and giving it as input of another pipe.
	/// </summary>
	/// <typeparam name="T">The type of the data.</typeparam>
	public interface IPipeConnector<T>
	{
		/// <summary>
		/// Processes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		void Process(IList<T> data);
	}
}