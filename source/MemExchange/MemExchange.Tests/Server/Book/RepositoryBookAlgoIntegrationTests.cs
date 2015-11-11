using MemExchange.Core.SharedDto;
using MemExchange.Server.Common;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server.Book
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
            var limitAlgo = new LimitOrderMatchingAlgorithm(new DateService());
            var marketAlgo = new MarketOrderMatchingAlgorithm(new DateService());
            var level1 = new OrderBookBestBidAsk("ABC");
            var book = new OrderBook("ABC", limitAlgo, marketAlgo, level1);

            var sellOrder1 = repo.NewLimitOrder("ABC", 9, 88.2d, 100, WayEnum.Sell);
            book.AddLimitOrder(sellOrder1);

            var buyOrder1 = repo.NewLimitOrder("ABC", 9, 88.0d, 50, WayEnum.Buy);
            book.AddLimitOrder(buyOrder1);

            Assert.AreEqual(88.2d, level1.BestAskPrice);
            Assert.AreEqual(88.0d, level1.BestBidPrice);

            buyOrder1.Modify(50, 88.2d);

            Assert.AreEqual(88.2d, level1.BestAskPrice);
            Assert.IsNull(level1.BestBidPrice);

            var retrievedBuyOrder = repo.TryGetLimitOrder(buyOrder1.ExchangeOrderId);
            Assert.IsNull(retrievedBuyOrder);

        }
    }
}
