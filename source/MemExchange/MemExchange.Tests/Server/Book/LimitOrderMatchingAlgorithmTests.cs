using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Server.Common;
using MemExchange.Server.Processor.Book.Executions;
using MemExchange.Server.Processor.Book.MatchingAlgorithms;
using MemExchange.Server.Processor.Book.Orders;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server.Book
{
    
    [TestFixture]
    public class LimitOrderMatchingAlgorithmTests
    {
        private IDateService dateServiceMock;

        [SetUp]
        public void Setup()
        {
            dateServiceMock = MockRepository.GenerateMock<IDateService>();
        }

        [Test]
        public void ShouldNotMatchHigherSellOrderWithLowerBuyOrder()
        {
            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 10, 100, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 12);
            
            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(0, generatedExecutions.Count);
        }

        [Test]
        public void ShouldNotMatchOrdersWithDifferentSymbols()
        {
            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("QQQ", 10, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(0, generatedExecutions.Count);
        }

        [Test]
        public void ShouldNotMatchOrdersWithZeroQuantity()
        {
            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 0, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(0, generatedExecutions.Count);

            sellOrder.Modify(90);
            buyOrder.Modify(0);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(0, generatedExecutions.Count);
        }

        [Test]
        public void ShouldGenerateExecutionWithSamePriceAsBuyAndSell()
        {
            var staticDatetimeOffset = DateTimeOffset.UtcNow;
            dateServiceMock.Stub(a => a.UtcNow()).Return(staticDatetimeOffset);

            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 10, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(1, generatedExecutions.Count);

            Assert.AreEqual(10, generatedExecutions[0].MatchedQuantity);
            Assert.AreEqual(90, generatedExecutions[0].MatchedPrice);
            Assert.AreEqual(buyOrder, generatedExecutions[0].BuySideOrder);
            Assert.AreEqual(sellOrder, generatedExecutions[0].SellSideOrder);
            Assert.AreEqual(staticDatetimeOffset, generatedExecutions[0].ExecutionTime);
        }

        [Test]
        public void ShouldExecuteOnMidpointPrice()
        {
            var staticDatetimeOffset = DateTimeOffset.UtcNow;
            dateServiceMock.Stub(a => a.UtcNow()).Return(staticDatetimeOffset);

            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 10, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 10, 91, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(1, generatedExecutions.Count);

            Assert.AreEqual(10, generatedExecutions[0].MatchedQuantity);
            Assert.AreEqual(90.5d, generatedExecutions[0].MatchedPrice);
            Assert.AreEqual(buyOrder, generatedExecutions[0].BuySideOrder);
            Assert.AreEqual(sellOrder, generatedExecutions[0].SellSideOrder);
            Assert.AreEqual(staticDatetimeOffset, generatedExecutions[0].ExecutionTime);
        }

        [Test]
        public void ShouldExecuteOnLowestQuantity()
        {
            var staticDatetimeOffset = DateTimeOffset.UtcNow;
            dateServiceMock.Stub(a => a.UtcNow()).Return(staticDatetimeOffset);

            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 40, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 100, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(1, generatedExecutions.Count);
            Assert.AreEqual(40, generatedExecutions[0].MatchedQuantity);
        }

        [Test]
        public void OrdersShouldHaveModifiedQuantitiesAfterMatch()
        {
            var staticDatetimeOffset = DateTimeOffset.UtcNow;
            dateServiceMock.Stub(a => a.UtcNow()).Return(staticDatetimeOffset);

            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 40, 90, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 100, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(0, sellOrder.Quantity);
            Assert.AreEqual(60, buyOrder.Quantity);
        }
        
        [Test]
        public void OrdersShouldNotHaveModifiedQuantitiesAfterNoMatch()
        {
            var staticDatetimeOffset = DateTimeOffset.UtcNow;
            dateServiceMock.Stub(a => a.UtcNow()).Return(staticDatetimeOffset);

            var generatedExecutions = new List<INewExecution>();
            var algo = new LimitOrderMatchingAlgorithm(dateServiceMock);
            algo.AddExecutionsHandler(generatedExecutions.Add);

            ILimitOrder sellOrder = new LimitOrder("ABC", 40, 91, WayEnum.Sell, 13);
            ILimitOrder buyOrder = new LimitOrder("ABC", 100, 90, WayEnum.Buy, 12);

            algo.TryMatch(buyOrder, sellOrder);
            Assert.AreEqual(40, sellOrder.Quantity);
            Assert.AreEqual(100, buyOrder.Quantity);
        }

        
        
    }
}
