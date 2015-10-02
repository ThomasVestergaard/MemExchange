using System.Threading;
using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;
using MemExchange.Server.Outgoing;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class OutgoingQueueTests
    {

        private IMessagePublisher messagePublisherMock;
        private ILogger loggerMock;
        private IOutgoingQueue outgoingQueue;

        [SetUp]
        public void Setup()
        {
            messagePublisherMock = MockRepository.GenerateMock<IMessagePublisher>();
            loggerMock = MockRepository.GenerateMock<ILogger>();
            outgoingQueue = new OutgoingQueue(loggerMock, messagePublisherMock);
            outgoingQueue.Start();
        }

        [TearDown]
        public void Teardown()
        {
            outgoingQueue.Stop();
        }

        [Test]
        public void PublisherShouldReceiveOrderAddedData()
        {
            var limitOrder = new LimitOrder
            {
                ClientId = 90,
                ExchangeOrderId = 11,
                Price = 20d,
                Quantity = 21,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            outgoingQueue.EnqueueAddedLimitOrder(limitOrder);

            Thread.Sleep(100);
            messagePublisherMock.AssertWasCalled(a => a.OnNext(Arg<ServerToClientMessage>.Matches(b => 
                b.MessageType == ServerToClientMessageTypeEnum.OrderAccepted
                && b.LimitOrder.ClientId == 90
                && b.LimitOrder.ExchangeOrderId == 11
                && b.LimitOrder.Price == 20d
                && b.LimitOrder.Quantity == 21
                && b.LimitOrder.Symbol == "ABC"
                && b.LimitOrder.Way == WayEnum.Buy),
                Arg<long>.Is.Anything,
                Arg<bool>.Is.Anything), options => options.Repeat.Once());
        }

        [Test]
        public void PublisherShouldReceiveOrderModifiedData()
        {
            var limitOrder = new LimitOrder
            {
                ClientId = 90,
                ExchangeOrderId = 11,
                Price = 20d,
                Quantity = 21,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            outgoingQueue.EnqueueUpdatedLimitOrder(limitOrder);

            Thread.Sleep(100);
            messagePublisherMock.AssertWasCalled(a => a.OnNext(Arg<ServerToClientMessage>.Matches(b =>
                b.MessageType == ServerToClientMessageTypeEnum.OrderChanged
                && b.LimitOrder.ClientId == 90
                && b.LimitOrder.ExchangeOrderId == 11
                && b.LimitOrder.Price == 20d
                && b.LimitOrder.Quantity == 21
                && b.LimitOrder.Symbol == "ABC"
                && b.LimitOrder.Way == WayEnum.Buy),
                Arg<long>.Is.Anything,
                Arg<bool>.Is.Anything), options => options.Repeat.Once());
        }

        [Test]
        public void PublisherShouldReceiveOrderDeletedData()
        {
            var limitOrder = new LimitOrder
            {
                ClientId = 90,
                ExchangeOrderId = 11,
                Price = 20d,
                Quantity = 21,
                Symbol = "ABC",
                Way = WayEnum.Buy
            };

            outgoingQueue.EnqueueDeletedLimitOrder(limitOrder);

            Thread.Sleep(100);
            messagePublisherMock.AssertWasCalled(a => a.OnNext(Arg<ServerToClientMessage>.Matches(b =>
                b.MessageType == ServerToClientMessageTypeEnum.OrderDeleted
                && b.LimitOrder.ClientId == 90
                && b.LimitOrder.ExchangeOrderId == 11
                && b.LimitOrder.Price == 20d
                && b.LimitOrder.Quantity == 21
                && b.LimitOrder.Symbol == "ABC"
                && b.LimitOrder.Way == WayEnum.Buy),
                Arg<long>.Is.Anything,
                Arg<bool>.Is.Anything), options => options.Repeat.Once());
        }

    }
}
