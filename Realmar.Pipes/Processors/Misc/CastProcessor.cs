using System;

namespace Realmar.Pipes.Processors.Misc
{
	/// <summary>
	/// The CastProcessor class.
	///  Casts the input put data to the specified type.
	/// </summary>
	/// <typeparam name="T">The type to which the input data should be cast.</typeparam>
	/// <seealso cref="object" />
	public class CastProcessor<T> : IPipeProcessor<object, T>
	{
		/// <inheritdoc />
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="OverflowException"></exception>
		/// <exception cref="FormatException"></exception>
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