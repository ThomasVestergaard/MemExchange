using MemExchange.Core.SharedDto;
using MemExchange.Server.Common;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server.NewBook
{
    [TestFixture]
    public class RepositoryBookAlgoIntegrationTests
    {
        private IOutgoingQueue outgoingQueueMock;

        [SetUp]
        public void Setup()
        {
            outgoingQueueMock = MockRepository.GenerateMock<IOutgoingQueue>();
        }

        [Test]
        public void ShouldCreateNewOrderBookItMatchItAndRemoveIt()
        {
            var repo = new OrderRepository();
            var algo = new LimitOrderMatchingAlgorithm(new DateService());
            var level1 = new OrderBookBestBidAsk("ABC");
            var book = new OrderBook("ABC", algo, level1, outgoingQueueMock);

            var sellOrder1 = repo.NewLimitOrder("ABC", 9, 88.2d, 100, WayEnum.Sell);
            book.HandleOrder(sellOrder1);

            var buyOrder1 = repo.NewLimitOrder("ABC", 9, 88.0d, 50, WayEnum.Buy);
            book.HandleOrder(buyOrder1);

            Assert.AreEqual(88.2d, level1.BestAskPrice);
            Assert.AreEqual(88.0d, level1.BestBidPrice);

            buyOrder1.Modify(50, 88.2d);

            Assert.AreEqual(88.2d, level1.BestAskPrice);
            Assert.IsNull(level1.BestBidPrice);

            var retrievedBuyOrder = repo.TryGetOrder(buyOrder1.ExchangeOrderId);
            Assert.IsNull(retrievedBuyOrder);

        }
    }
}
