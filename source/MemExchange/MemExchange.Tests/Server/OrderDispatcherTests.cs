using MemExchange.Core.Logging;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Clients;
using MemExchange.Server.Common;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor;
using MemExchange.Server.Processor.Book;
using NUnit.Framework;
using Rhino.Mocks;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class OrderDispatcherTests
    {
        private IOutgoingQueue outgoingQueueMock;
        private IOrderRepository orderRepositoryMock;
        private ILogger loggerMock;

        [SetUp]
        public void Setup()
        {
            outgoingQueueMock = MockRepository.GenerateMock<IOutgoingQueue>();
            orderRepositoryMock = MockRepository.GenerateMock<IOrderRepository>();
            loggerMock = MockRepository.GenerateMock<ILogger>();
        }

       
       

    }
}
