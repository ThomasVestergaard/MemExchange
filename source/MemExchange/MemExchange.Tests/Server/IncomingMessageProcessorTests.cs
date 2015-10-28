using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Common;
using MemExchange.Server.Incoming;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor;
using MemExchange.Server.Processor.Book;
using MemExchange.Server.Processor.Book.Orders;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class IncomingMessageProcessorTests
    {
        private IOrderRepository ordereRepositoryMock;
        private IOutgoingQueue outgoingQueueMock;
        private IDateService dateServiceMock;
        private IOrderDispatcher orderDispatcherMock;

        [SetUp]
        public void Setup()
        {
            ordereRepositoryMock = MockRepository.GenerateMock<IOrderRepository>();
            outgoingQueueMock = MockRepository.GenerateMock<IOutgoingQueue>();
            dateServiceMock = MockRepository.GenerateMock<IDateService>();
            orderDispatcherMock = MockRepository.GenerateMock<IOrderDispatcher>();
        }

        [Test]
        public void ShouldNotCallDispatcherIfOrderDoesNotValidate()
        {
            var processor = new IncomingMessageProcessor(ordereRepositoryMock, outgoingQueueMock, dateServiceMock, orderDispatcherMock);

            var invalidLimitOrder = new LimitOrderDto();
            invalidLimitOrder.Reeset();

            processor.OnNext(new ClientToServerMessageQueueItem
            {
                Message = new ClientToServerMessage
                {
                    ClientId = 1,
                    LimitOrder = invalidLimitOrder,
                    MessageType = ClientToServerMessageTypeEnum.PlaceOrder
                },
            }, 1, true);

            orderDispatcherMock.AssertWasNotCalled(a => a.HandleAddOrder(Arg<ILimitOrder>.Is.Anything));
            outgoingQueueMock.AssertWasNotCalled(a => a.EnqueueAddedLimitOrder(Arg<ILimitOrder>.Is.Anything));
        }

        [Test]
        public void ShouldCallDispatcherWhenLimitOrderValidates()
        {
            var processor = new IncomingMessageProcessor(ordereRepositoryMock, outgoingQueueMock, dateServiceMock, orderDispatcherMock);

            var limitOrder = new LimitOrderDto();
            limitOrder.Reeset();
            limitOrder.Symbol = "QQQ";
            limitOrder.Price = 30;
            limitOrder.Quantity = 10;
            limitOrder.ClientId = 1;
            limitOrder.Way = WayEnum.Sell;

            processor.OnNext(new ClientToServerMessageQueueItem
            {
                Message =

                    new ClientToServerMessage
                    {
                        ClientId = 1,
                        LimitOrder = limitOrder,
                        MessageType = ClientToServerMessageTypeEnum.PlaceOrder
                    }
            }, 1, true);

            orderDispatcherMock.AssertWasCalled(a => a.HandleAddOrder(Arg<ILimitOrder>.Is.Equal(limitOrder)));
        }

        [Test]
        public void ShouldNotCallDispatcherWhenLimitOrderIsInvalid()
        {
            var processor = new IncomingMessageProcessor(ordereRepositoryMock, outgoingQueueMock, dateServiceMock, orderDispatcherMock);

            var limitOrder = new LimitOrderDto();
            limitOrder.Reeset();
            limitOrder.Symbol = "QQQ";
            limitOrder.Price = -1;
            limitOrder.Quantity = -1;
            limitOrder.ClientId = 1;
            limitOrder.Way = WayEnum.Sell;

            processor.OnNext(
                new ClientToServerMessageQueueItem
                {
                    Message =

                        new ClientToServerMessage
                        {
                            ClientId = 1,
                            LimitOrder = limitOrder,
                            MessageType = ClientToServerMessageTypeEnum.PlaceOrder
                        }
                }, 1, true);

            orderDispatcherMock.AssertWasNotCalled(a => a.HandleAddOrder(Arg<ILimitOrder>.Is.Anything));
        }

        
        [Test]
        public void ShouldNotCallDispatcherWhenLimitOrderIsInvalidOnCancelOrder()
        {
            var processor = new IncomingMessageProcessor(ordereRepositoryMock, outgoingQueueMock, dateServiceMock, orderDispatcherMock);

            var limitOrder = new LimitOrderDto();
            limitOrder.Reeset();

            processor.OnNext(
                new ClientToServerMessageQueueItem
                {
                    Message = 
                    new ClientToServerMessage
                    {
                        ClientId = 1,
                        LimitOrder = limitOrder,
                        MessageType = ClientToServerMessageTypeEnum.CancelOrder
                    }
                }, 1, true);

            orderDispatcherMock.AssertWasNotCalled(a => a.HandleAddOrder(Arg<ILimitOrder>.Is.Anything));
        }

       
    }
}
