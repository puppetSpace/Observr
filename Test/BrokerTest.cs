using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Observr;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
	[TestClass]
	public class BrokerTest
	{
		[TestMethod]
		public async Task Subscribe_GetsNotifiedOnChanges()
		{
			string changedObject = null;
			var moqObserver = new Mock<IObserver<string>>();
			moqObserver.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => changedObject = s);

			var broker = new Broker();
			broker.Subscribe(moqObserver.Object);
			await broker.Publish("Test");

			Assert.AreEqual("Test", changedObject);

		}

		[TestMethod]
		public async Task Subscribe_GetsNotifiedOnlyOnce()
		{
			int calls = 0;
			var moqObserver = new Mock<IObserver<string>>();
			moqObserver.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => calls++); ;

			var broker = new Broker();
			broker.Subscribe(moqObserver.Object);
			await broker.Publish("Test");

			Assert.AreEqual(1, calls);

		}

		[TestMethod]
		public async Task Subscribe_SameObserverTwice_OnlyGetsAddedOnce()
		{
			int calls = 0;
			var moqObserver = new Mock<IObserver<string>>();
			moqObserver.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => calls++); ;

			var broker = new Broker();
			broker.Subscribe(moqObserver.Object);
			broker.Subscribe(moqObserver.Object);
			await broker.Publish("Test");

			Assert.AreEqual(1, calls);

		}

		[TestMethod]
		public async Task Subscribe_DifferentObservers_EachGetCalledOnce()
		{
			int callsString = 0;
			int callsInt = 0;
			var moqObserver1 = new Mock<IObserver<string>>();
			moqObserver1.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => callsString++);

			var moqObserver2 = new Mock<IObserver<int>>();
			moqObserver2.Setup(x => x.Handle(It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.Callback((int s, CancellationToken c) => callsInt++); ;

			var broker = new Broker();
			broker.Subscribe(moqObserver1.Object);
			broker.Subscribe(moqObserver2.Object);
			await broker.Publish("Test");
			await broker.Publish(1);

			Assert.AreEqual(1, callsString);
			Assert.AreEqual(1, callsInt);
		}

		[TestMethod]
		public async Task Subscribe_Unsubscribed_DoesntGetCalled()
		{
			int calls = 0;
			var moqObserver = new Mock<IObserver<string>>();
			moqObserver.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => calls++); ;

			var broker = new Broker();
			var subscription = broker.Subscribe(moqObserver.Object);
			await broker.Publish("Test");

			subscription.Dispose();

			await broker.Publish("Test");

			Assert.AreEqual(1, calls);
		}

		[TestMethod]
		public async Task Subscribe_DifferentObservers_UnsubscribeOne_OtherStillGetsCalled()
		{
			int callsString = 0;
			int callsInt = 0;
			var moqObserver1 = new Mock<IObserver<string>>();
			moqObserver1.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Callback((string s, CancellationToken c) => callsString++);

			var moqObserver2 = new Mock<IObserver<int>>();
			moqObserver2.Setup(x => x.Handle(It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.Callback((int s, CancellationToken c) => callsInt++); ;

			var broker = new Broker();
			var subscription = broker.Subscribe(moqObserver1.Object);
			broker.Subscribe(moqObserver2.Object);
			await broker.Publish("Test");
			subscription.Dispose();
			await broker.Publish("Test");
			await broker.Publish(1);
			await broker.Publish(1);

			Assert.AreEqual(1, callsString);
			Assert.AreEqual(2, callsInt);
		}
	}
}
