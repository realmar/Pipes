using Xunit;

namespace Realmar.Pipes.Tests.Unit.Pipes
{
	public class NonBlockingPipeTests
	{
		[Fact]
		public void Dispose_DoNotThrowWhenCalledTwice()
		{
			var pipe = new NonBlockingPipe<object>();
			pipe.Dispose();

			// implicit do not throw assertion
			pipe.Dispose();
		}
	}
}