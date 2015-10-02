using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ServerToClient;
using NUnit.Framework;

namespace MemExchange.Tests.Core
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void ShouldSerializeServerToClientMessage()
        {
            var serializer = new ProtobufSerializer();
            var s = serializer.Serialize(new ServerToClientMessage { MessageType = ServerToClientMessageTypeEnum.OrderAccepted});

            Assert.IsNotNull(s);

            var d = serializer.Deserialize<ServerToClientMessage>(s);
            Assert.IsNotNull(d);
        }

    }
}
