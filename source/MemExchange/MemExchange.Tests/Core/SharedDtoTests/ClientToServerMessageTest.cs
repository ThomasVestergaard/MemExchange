using MemExchange.Core.SharedDto.ClientToServer;
using NUnit.Framework;

namespace MemExchange.Tests.Core.SharedDtoTests
{
    [TestFixture]
    public class ClientToServerMessageTest
    {

        [Test]
        public void ShouldUpdateData()
        {
            var original = new ClientToServerMessage();
            original.ClientId = 99;
            original.MessageType = ClientToServerMessageTypeEnum.CancelLimitOrder;

            var newMessage = new ClientToServerMessage();
            newMessage.ClientId = 50;
            newMessage.MessageType = ClientToServerMessageTypeEnum.RequestOpenOrders;

            original.Update(newMessage);

            Assert.AreEqual(original.ClientId, newMessage.ClientId);
            Assert.AreEqual(original.MessageType, newMessage.MessageType);
        }
    }
}
