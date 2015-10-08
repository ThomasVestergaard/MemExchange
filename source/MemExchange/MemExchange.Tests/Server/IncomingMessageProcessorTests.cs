using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class IncomingMessageProcessorTests
    {
        private IOrderKeep orderKeepMock;
        private IOutgoingQueue outgoingQueueMock;

        [SetUp]
        public void Setup()
        {
            orderKeepMock = MockRepository.GenerateMock<IOrderKeep>();
            outgoingQueueMock = MockRepository.GenerateMock<IOutgoingQueue>();
        }

        [Test]
        public void ShouldNotCallOrderKeepIfOrderDoesNotValidate()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);

            var invalidLimitOrder = new LimitOrder();
            invalidLimitOrder.Reeset();

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = invalidLimitOrder,
                MessageType = ClientToServerMessageTypeEnum.PlaceOrder
            }, 1, true);

            orderKeepMock.AssertWasNotCalled(a => a.AddLimitOrder(Arg<LimitOrder>.Is.Anything, out Arg<LimitOrder>.Out(new LimitOrder()).Dummy));
            outgoingQueueMock.AssertWasNotCalled(a => a.EnqueueAddedLimitOrder(Arg<LimitOrder>.Is.Anything));
        }

        [Test]
        public void ShouldCallOrderKeepWhenLimitOrderValidates()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);

            var limitOrder = new LimitOrder();
            limitOrder.Reeset();
            limitOrder.Symbol = "QQQ";
            limitOrder.Price = 30;
            limitOrder.Quantity = 10;
            limitOrder.ClientId = 1;
            limitOrder.Way = WayEnum.Sell;

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = limitOrder,
                MessageType = ClientToServerMessageTypeEnum.PlaceOrder
            }, 1, true);

            orderKeepMock.AssertWasCalled(a => a.AddLimitOrder(Arg<LimitOrder>.Is.Equal(limitOrder), out Arg<LimitOrder>.Out(new LimitOrder()).Dummy));
        }

        [Test]
        public void ShouldNotCallOrderKeepWhenLimitOrderIsInvalid()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);

            var limitOrder = new LimitOrder();
            limitOrder.Reeset();
            limitOrder.Symbol = "QQQ";
            limitOrder.Price = -1;
            limitOrder.Quantity = -1;
            limitOrder.ClientId = 1;
            limitOrder.Way = WayEnum.Sell;

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = limitOrder,
                MessageType = ClientToServerMessageTypeEnum.PlaceOrder
            }, 1, true);

            orderKeepMock.AssertWasNotCalled(a => a.AddLimitOrder(Arg<LimitOrder>.Is.Anything, out Arg<LimitOrder>.Out(new LimitOrder()).Dummy));
        }

        [Test]
        public void OutputQueueShouldBeCalledWithOutputFromOrderKeepOnValidLimitOrder()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);
            
            var orderKeepReturnOrder = new LimitOrder();
            orderKeepReturnOrder.Reeset();
            orderKeepReturnOrder.Symbol = "QQQ";
            orderKeepReturnOrder.Price = 30;
            orderKeepReturnOrder.Quantity = 10;
            orderKeepReturnOrder.ClientId = 1;
            orderKeepReturnOrder.Way = WayEnum.Sell;
            

            var limitOrder = new LimitOrder();
            limitOrder.Reeset();
            limitOrder.Symbol = "QQQ";
            limitOrder.Price = 30;
            limitOrder.Quantity = 10;
            limitOrder.ClientId = 1;
            limitOrder.Way = WayEnum.Sell;

            LimitOrder addedOrder;
            orderKeepMock.Stub(a => a.AddLimitOrder(limitOrder, out addedOrder)).OutRef(orderKeepReturnOrder);

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = limitOrder,
                MessageType = ClientToServerMessageTypeEnum.PlaceOrder
            }, 1, true);

            outgoingQueueMock.AssertWasCalled(a => a.EnqueueAddedLimitOrder(Arg<LimitOrder>.Is.Equal(orderKeepReturnOrder)));
        }

        [Test]
        public void ShouldNotCallOrderKeepWhenLimitOrderIsInvalidOnCancelOrder()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);

            var limitOrder = new LimitOrder();
            limitOrder.Reeset();

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = limitOrder,
                MessageType = ClientToServerMessageTypeEnum.CancelOrder
            }, 1, true);

            orderKeepMock.AssertWasNotCalled(a => a.AddLimitOrder(Arg<LimitOrder>.Is.Anything, out Arg<LimitOrder>.Out(new LimitOrder()).Dummy));
        }

        [Test]
        public void ShouldCallOrderKeepWhenLimitOrderIsValidOnCancelOrder()
        {
            var processor = new IncomingMessageProcessor(orderKeepMock, outgoingQueueMock);

            var limitOrder = new LimitOrder();
            limitOrder.Reeset();
            limitOrder.ClientId = 1;
            limitOrder.ExchangeOrderId = 50;

            processor.OnNext(new ClientToServerMessage
            {
                ClientId = 1,
                LimitOrder = limitOrder,
                MessageType = ClientToServerMessageTypeEnum.CancelOrder
            }, 1, true);

            orderKeepMock.AssertWasCalled(a => a.DeleteLimitOrder(Arg<LimitOrder>.Is.Equal(limitOrder)));
        }
    }
}
