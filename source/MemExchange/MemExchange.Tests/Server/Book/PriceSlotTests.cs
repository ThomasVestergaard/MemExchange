using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Common;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;
using NUnit.Framework;

namespace MemExchange.Tests.Server.Book
{
    [TestFixture]
    public class PriceSlotTests
    {
        [Test]
        public void ShouldAddBuyOrder()
        {
            IPriceSlot priceSlot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            var buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 9);
            priceSlot.AddOrder(buyOrder);

            Assert.AreEqual(1, priceSlot.BuyOrders.Count);
            Assert.AreEqual(buyOrder, priceSlot.BuyOrders[0]);
        }

        [Test]
        public void ShouldAddSellOrder()
        {
            IPriceSlot priceSlot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            var sellOrder = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 9);
            priceSlot.AddOrder(sellOrder);

            Assert.AreEqual(1, priceSlot.SellOrders.Count);
            Assert.AreEqual(sellOrder, priceSlot.SellOrders[0]);
        }

        [Test]
        public void ShouldRemoveBuyOrder()
        {
            IPriceSlot priceSlot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            var buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 9);
            priceSlot.AddOrder(buyOrder);

            Assert.AreEqual(1, priceSlot.BuyOrders.Count);
            Assert.AreEqual(buyOrder, priceSlot.BuyOrders[0]);

            priceSlot.RemoveOrder(buyOrder);
            Assert.AreEqual(0, priceSlot.BuyOrders.Count);
        }

        [Test]
        public void ShouldRemoveSellOrder()
        {
            IPriceSlot priceSlot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            var sellOrder = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 9);
            priceSlot.AddOrder(sellOrder);

            Assert.AreEqual(1, priceSlot.SellOrders.Count);
            Assert.AreEqual(sellOrder, priceSlot.SellOrders[0]);

            priceSlot.RemoveOrder(sellOrder);

            Assert.AreEqual(0, priceSlot.SellOrders.Count);
        }

        [Test]
        public void ShouldIgnoreOrderWithDifferentPriceOnAdd()
        {
            IPriceSlot priceSlot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            var buyOrder = new LimitOrder("ABC", 10, 91, WayEnum.Buy, 9);
            priceSlot.AddOrder(buyOrder);

            Assert.AreEqual(0, priceSlot.BuyOrders.Count);
        }

        [Test]
        public void ShouldMatchBuyOrderCompletely()
        {
            var executions = new List<INewExecution>();
            var executionalgo = new LimitOrderMatchingAlgorithm(new DateService());
            executionalgo.AddExecutionsHandler(executions.Add);



            var priceSlot = new PriceSlot(90, executionalgo, new MarketOrderMatchingAlgorithm(new DateService()));

            var sellOrder = new LimitOrder("ABC", 100, 90, WayEnum.Sell, 90);
            var buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 80);

            priceSlot.AddOrder(sellOrder);
            priceSlot.TryMatchLimitOrder(buyOrder);

            Assert.AreEqual(1, executions.Count);
            Assert.AreEqual(0, priceSlot.BuyOrders.Count);
            Assert.AreEqual(1, priceSlot.SellOrders.Count);
            Assert.AreEqual(90, priceSlot.SellOrders[0].Quantity);
        }

        [Test]
        public void ShouldMatchMultipleSellOrdersToSameBuyOrder()
        {
            var executions = new List<INewExecution>();
            var executionalgo = new LimitOrderMatchingAlgorithm(new DateService());
            executionalgo.AddExecutionsHandler(executions.Add);

            var priceSlot = new PriceSlot(90, executionalgo, new MarketOrderMatchingAlgorithm(new DateService()));

            var sellOrder1 = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 90);
            var sellOrder2 = new LimitOrder("ABC", 40, 90, WayEnum.Sell, 90);
            var buyOrder = new LimitOrder("ABC", 50, 90, WayEnum.Buy, 80);

            priceSlot.AddOrder(sellOrder1);
            priceSlot.AddOrder(sellOrder2);
            priceSlot.TryMatchLimitOrder(buyOrder);

            Assert.AreEqual(2, executions.Count);
            Assert.AreEqual(0, priceSlot.BuyOrders.Count);
            Assert.AreEqual(0, priceSlot.SellOrders.Count);
            
        }

        [Test]
        public void ShouldReturnFalseWhenNoOrdersArePresent()
        {
            var slot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));
            Assert.IsFalse(slot.HasOrders);
        }

        [Test]
        public void ShouldReturnTrueWhenBuyOrdersArePresent()
        {
            var slot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));

            var buyOrder = new LimitOrder("ABC", 50, 90, WayEnum.Buy, 80);
            slot.AddOrder(buyOrder);
            
            Assert.IsTrue(slot.HasOrders);
            Assert.IsTrue(slot.HasBids);
            Assert.IsFalse(slot.HasAsks);
        }

        [Test]
        public void ShouldReturnTrueWhenSellOrdersArePresent()
        {
            var slot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));

            var sellOrder = new LimitOrder("ABC", 50, 90, WayEnum.Sell, 80);
            slot.AddOrder(sellOrder);

            Assert.IsTrue(slot.HasOrders);
            Assert.IsTrue(slot.HasAsks);
            Assert.IsFalse(slot.HasBids);
        }

        [Test]
        public void ShouldReturnTrueWhenBuyAndSellOrdersArePresent()
        {
            var slot = new PriceSlot(90, new LimitOrderMatchingAlgorithm(new DateService()), new MarketOrderMatchingAlgorithm(new DateService()));

            var buyOrder = new LimitOrder("ABC", 50, 90, WayEnum.Buy, 80);
            var sellOrder = new LimitOrder("ABC", 50, 90, WayEnum.Sell, 80);
            slot.AddOrder(buyOrder);
            slot.AddOrder(sellOrder);

            Assert.IsTrue(slot.HasOrders);
        }
    }
}
