using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Books;
using NUnit.Framework;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class OrderBookSlotTests
    {

        [Test]
        public void ShouldNotAddOrderWithDifferentWay()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Sell, Price = 10, Quantity = 10});
            Assert.AreEqual(0, slot.Orders.Count);
        }

        [Test]
        public void ShouldNotAddOrderWithDifferentPrice()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 11, Quantity = 10 });
            Assert.AreEqual(0, slot.Orders.Count);
        }

        [Test]
        public void ShouldAddOrderWithSameWayAndPrice()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 10, Quantity = 10 });
            Assert.AreEqual(1, slot.Orders.Count);
            Assert.AreEqual(10, slot.TotalQuantity);
        }

        [Test]
        public void ShouldUpdateTotalQuantityOnAddOrder()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 10, Quantity = 10 });
            Assert.AreEqual(10, slot.TotalQuantity);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 10, Quantity = 5 });
            Assert.AreEqual(15, slot.TotalQuantity);
        }

        [Test]
        public void ShouldNotExecuteOnOrderWithInvalidWay()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 10, Quantity = 10 });
            var sellOrder = new LimitOrder {Way = WayEnum.Buy, Price = 10, Quantity = 10};

            var matchResult = slot.MatchOrder(sellOrder);
            Assert.AreEqual(0, matchResult.Count);
        }

        [Test]
        public void ShouldNotExecuteOnOrderWithInvalidPrice()
        {
            var slot = new OrderBookSlot(10, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            slot.AddOrder(new LimitOrder { Way = WayEnum.Buy, Price = 10, Quantity = 10 });
            var sellOrder = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 10 };

            var matchResult = slot.MatchOrder(sellOrder);
            Assert.AreEqual(0, matchResult.Count);
        }

        [Test]
        public void ShouldGenerateExecutionOnSameQuantityWithZeroRestQuantity_SellOrder()
        {
            var slot = new OrderBookSlot(12, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            var buyOrder = new LimitOrder {Way = WayEnum.Buy, Price = 12, Quantity = 10};
            var sellOrder = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 10 };

            slot.AddOrder(buyOrder);
            var matchResult = slot.MatchOrder(sellOrder);
            Assert.AreEqual(1, matchResult.Count);
            Assert.AreEqual(0, slot.TotalQuantity);
            Assert.AreEqual(0, slot.Orders.Count);
            Assert.AreEqual(buyOrder, matchResult[0].BuySideOrder);
            Assert.AreEqual(sellOrder, matchResult[0].SellSideOrder);
            Assert.AreEqual(10, matchResult[0].MatchedQuantity);
            Assert.AreEqual(12, matchResult[0].MatchedPrice);
        }

        [Test]
        public void ShouldGenerateExecutionPerMatchedOrderInBook_SellOrder()
        {
            var slot = new OrderBookSlot(12, WayEnum.Buy);
            Assert.AreEqual(0, slot.Orders.Count);

            var buyOrder = new LimitOrder { Way = WayEnum.Buy, Price = 12, Quantity = 10 };
            var buyOrder2 = new LimitOrder { Way = WayEnum.Buy, Price = 12, Quantity = 25 };
            var sellOrder = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 21 };

            slot.AddOrder(buyOrder);
            slot.AddOrder(buyOrder2);
            var matchResult = slot.MatchOrder(sellOrder);
            
            Assert.AreEqual(2, matchResult.Count);
            Assert.AreEqual(14, slot.TotalQuantity);
            Assert.AreEqual(1, slot.Orders.Count);
            Assert.AreEqual(14, slot.Orders[0].Quantity);

            Assert.AreEqual(buyOrder, matchResult[0].BuySideOrder);
            Assert.AreEqual(sellOrder, matchResult[0].SellSideOrder);
            Assert.AreEqual(buyOrder2, matchResult[1].BuySideOrder);
            Assert.AreEqual(sellOrder, matchResult[1].SellSideOrder);

            Assert.AreEqual(10, matchResult[0].MatchedQuantity);
            Assert.AreEqual(12, matchResult[0].MatchedPrice);

            Assert.AreEqual(11, matchResult[1].MatchedQuantity);
            Assert.AreEqual(12, matchResult[1].MatchedPrice);
        }

        [Test]
        public void ShouldGenerateExecutionOnSameQuantityWithZeroRestQuantity_BuyOrder()
        {
            var slot = new OrderBookSlot(12, WayEnum.Sell);
            Assert.AreEqual(0, slot.Orders.Count);

            var sellOrder = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 10 };
            var buyOrder = new LimitOrder { Way = WayEnum.Buy, Price = 12, Quantity = 10 };

            slot.AddOrder(sellOrder);
            var matchResult = slot.MatchOrder(buyOrder);
            Assert.AreEqual(1, matchResult.Count);
            Assert.AreEqual(0, slot.TotalQuantity);
            Assert.AreEqual(0, slot.Orders.Count);
            Assert.AreEqual(sellOrder, matchResult[0].SellSideOrder);
            Assert.AreEqual(buyOrder, matchResult[0].BuySideOrder);
            Assert.AreEqual(10, matchResult[0].MatchedQuantity);
            Assert.AreEqual(12, matchResult[0].MatchedPrice);
        }

        [Test]
        public void ShouldGenerateExecutionPerMatchedOrderInBook_BuyOrder()
        {
            var slot = new OrderBookSlot(12, WayEnum.Sell);
            Assert.AreEqual(0, slot.Orders.Count);

            var sellOrder1 = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 10 };
            var sellOrder2 = new LimitOrder { Way = WayEnum.Sell, Price = 12, Quantity = 25 };
            var buyOrder = new LimitOrder { Way = WayEnum.Buy, Price = 12, Quantity = 21 };

            slot.AddOrder(sellOrder1);
            slot.AddOrder(sellOrder2);
            var matchResult = slot.MatchOrder(buyOrder);

            Assert.AreEqual(2, matchResult.Count);
            Assert.AreEqual(14, slot.TotalQuantity);
            Assert.AreEqual(1, slot.Orders.Count);
            Assert.AreEqual(14, slot.Orders[0].Quantity);

            Assert.AreEqual(sellOrder1, matchResult[0].SellSideOrder);
            Assert.AreEqual(buyOrder, matchResult[0].BuySideOrder);
            Assert.AreEqual(sellOrder2, matchResult[1].SellSideOrder);
            Assert.AreEqual(buyOrder, matchResult[1].BuySideOrder);

            Assert.AreEqual(10, matchResult[0].MatchedQuantity);
            Assert.AreEqual(12, matchResult[0].MatchedPrice);

            Assert.AreEqual(11, matchResult[1].MatchedQuantity);
            Assert.AreEqual(12, matchResult[1].MatchedPrice);
        }


    }
}
