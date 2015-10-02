using MemExchange.Core.SharedDto.ClientToServer;
using NUnit.Framework;

namespace MemExchange.Core.Test.SharedDtoTests
{
    [TestFixture]
    public class ClientToServerMessageTest
    {

        [Test]
        public void ShouldUpdateData()
        {
            var original = new ClientToServerMessage();
            original.ClientId = 99;
            original.MessageType = ClientToServerMessageTypeEnum.CancelOrder;
            original.Payload = "Hello there";

            var newMessage = new ClientToServerMessage();
            newMessage.ClientId = 50;
            newMessage.MessageType = ClientToServerMessageTypeEnum.RequestOpenOrders;
            newMessage.Payload = 99m;

            original.Update(newMessage);

            Assert.AreEqual(original.ClientId, newMessage.ClientId);
            Assert.AreEqual(original.MessageType, newMessage.MessageType);
            Assert.AreEqual(original.Payload, newMessage.Payload);
        }
    }
}
