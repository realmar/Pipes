using System.Diagnostics;
using Realmar.Pipes.Processors.Misc;
using Xunit;

namespace Realmar.Pipes.Tests.Unit.Processors.Misc
{
	public class WaitProcessorTests
	{
		[Fact]
		public void Process_CheckTime()
		{
			var waitTime = 500;
			var processor = new WaitProcessor<object>(waitTime);

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			processor.Process(new object());
			stopwatch.Stop();

			Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, waitTime, waitTime * 1.5);
		}
	}
}