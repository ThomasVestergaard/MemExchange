using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.ServerToClient;
using MemExchange.Server.Incoming;
using MemExchange.Tests.Tools;
using NUnit.Framework;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class RingbufferByteArrayTests
    {

        [Test]
        public void ShouldSetBufferLength()
        {
            var byteArray = new RingbufferByteArray();
            var source = new byte[] { 1,2,3,4,5,6,7,8,9,10 };

            Assert.AreEqual(0, byteArray.ContentLength);
            byteArray.Set(source);
            Assert.AreEqual(10, byteArray.ContentLength);
        }

        [Test]
        public void ShouldSetContent()
        {
            var byteArray = new RingbufferByteArray();
            var source = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] target = new byte[512];

            byteArray.Set(source);

            byteArray.GetContent(ref target);
            ArrayComparer.AreEqual(byteArray.ByteBuffer, target);
        }

        [Test]
        public void SerializeDeserializeTest()
        {
            var serializer = new ProtobufSerializer();
            var byteArray = new RingbufferByteArray();

            var item = new ServerToClientMessage();
            item.Message = "hello";
            item.MessageType = ServerToClientMessageTypeEnum.LimitOrderAccepted;

            byteArray.Set(serializer.Serialize(item));
            byte[] target = new byte[512];

            byteArray.GetContent(ref target);
            var deserialized = serializer.Deserialize<ServerToClientMessage>(target.Take(byteArray.ContentLength).ToArray());

            Assert.IsNotNull(deserialized);
            Assert.AreEqual("hello", deserialized.Message);
            Assert.AreEqual(ServerToClientMessageTypeEnum.LimitOrderAccepted, deserialized.MessageType);
        }
    }
}
