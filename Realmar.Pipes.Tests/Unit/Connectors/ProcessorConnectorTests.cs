using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Realmar.Pipes.Connectors;
using Realmar.Pipes.Tests.SampleProcessors.Math;
using Xunit;

namespace Realmar.Pipes.Tests.Unit.Connectors
{
	public class ProcessorConnectorTests
	{
		[Fact]
		public void EnsureProcessorIsNotGarbageCollected()
		{
			var multiplicator = 2d;
			var receivedData = new List<object>();

			var resultReceiverMock = new Mock<IResultReceiver>();
			resultReceiverMock
				.Setup(obj => obj.AddResult(It.IsAny<object>()))
				.Callback<object>(data => receivedData.Add(data));

			var connector = new ProcessorConnector<double>(resultReceiverMock.Object);
			connector.Connect(new MultiplicationProcessor(multiplicator))
				.Finish(results => { });

			// force GC
			GC.Collect();
			GC.WaitForPendingFinalizers();

			foreach (var i in Enumerable.Range(1, 8))
			{
				connector.Process(i);
				Assert.Equal(i * multiplicator, receivedData.Last());
			}
		}
	}
}