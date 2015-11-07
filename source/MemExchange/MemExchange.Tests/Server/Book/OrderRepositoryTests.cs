using MemExchange.Core.SharedDto;
using MemExchange.Server.Processor.Book;
using NUnit.Framework;

namespace MemExchange.Tests.Server.Book
{
    [TestFixture]
    public class OrderRepositoryTests
    {

        [Test]
        public void RepositoryShouldIncrementOrderNumber()
        {
            var repo = new OrderRepository();

            var firstOrder = repo.NewLimitOrder("ABC", 12, 200, 90, WayEnum.Buy);
            var secondOrder = repo.NewLimitOrder("ABC", 12, 200, 90, WayEnum.Buy);
            Assert.AreEqual(1, firstOrder.ExchangeOrderId);
            Assert.AreEqual(2, secondOrder.ExchangeOrderId);
        }

        [Test]
        public void RepositoryShouldReturnOrderWithCorrectData()
        {
            var repo = new OrderRepository();

            var order = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            Assert.AreEqual("ABC", order.Symbol);
            Assert.AreEqual(12, order.ClientId);
            Assert.AreEqual(200.45d, order.Price);
            Assert.AreEqual(90, order.Quantity);
            Assert.AreEqual(WayEnum.Buy, order.Way);
        }

        [Test]
        public void NewOrderShouldBeRetrievableThroughOrderId()
        {
            var repo = new OrderRepository();
            var order = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            var retrievedOrder = repo.TryGetLimitOrder(order.ExchangeOrderId);
            Assert.AreEqual(order, retrievedOrder);
        }

        [Test]
        public void RepositoryShouldReturnNullOnNonExistingOrderId()
        {
            var repo = new OrderRepository();
            var order = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            var retrievedOrder = repo.TryGetLimitOrder(666);
            Assert.IsNull(retrievedOrder);
        }

        [Test]
        public void RepositoryShouldRemoveOrderFromRepositoryWhenOrderIsDeleted()
        {
            var repo = new OrderRepository();
            var order = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            order.Delete();

            var retrievedOrder = repo.TryGetLimitOrder(order.ExchangeOrderId);
            Assert.IsNull(retrievedOrder);
        }

        [Test]
        public void ShouldReturnAllClientOrders()
        {
            var repo = new OrderRepository();

            var order1 = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            var order2 = repo.NewLimitOrder("ABC", 12, 200.45d, 90, WayEnum.Buy);
            var order3 = repo.NewLimitOrder("ABC", 13, 200.45d, 90, WayEnum.Buy);
            var order4 = repo.NewLimitOrder("ABC", 14, 200.45d, 90, WayEnum.Buy);

            var client12 = repo.GetClientLimitOrders(12);
            var client13 = repo.GetClientLimitOrders(13);
            var client14 = repo.GetClientLimitOrders(14);

            Assert.AreEqual(2, client12.Count);
            Assert.AreEqual(1, client13.Count);
            Assert.AreEqual(1, client14.Count);

            Assert.AreEqual(order1, client12[0]);
            Assert.AreEqual(order2, client12[1]);
            Assert.AreEqual(order3, client13[0]);
            Assert.AreEqual(order4, client14[0]);
        }

        [Test]
        public void RepositoryShouldRemoveStopLimitOrderWhenOrderIsDeleted()
        {
            var repo = new OrderRepository();
            var order = repo.NewStopLimitOrder("abc", 1, 12, 12, 100, WayEnum.Buy);
            order.Delete();
            var retrievedOrder = repo.TryGetStopLimitOrder(order.ExchangeOrderId);
            Assert.IsNull(retrievedOrder);

        }

    }
}
