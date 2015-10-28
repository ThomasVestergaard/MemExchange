
using System;
using System.Diagnostics;
using MemExchange.Core.Serialization;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;
using NUnit.Framework;

namespace MemExchange.Tests.Core
{
    [TestFixture]
    public class SerializationTests
    {

        [Test]
        public void ShouldSerializeAndDeserializeSeriesOfItems()
        {
            var serializer = new ProtobufSerializer();

            for (int i = 0; i < 1000; i++)
            {
                var item = new ServerToClientMessage();
                item.Message = i.ToString();
                item.MessageType = ServerToClientMessageTypeEnum.OrderAccepted;
                item.LimitOrder = new LimitOrderDto();
                item.LimitOrder.ClientId = i;
                item.LimitOrder.ExchangeOrderId = (uint)i;

                var serialized = serializer.Serialize(item);
                
                var deserialized = serializer.Deserialize<ServerToClientMessage>(serialized);
                
                Assert.IsNotNull(deserialized);
                Assert.AreEqual(i.ToString(), deserialized.Message );
                Assert.AreEqual(ServerToClientMessageTypeEnum.OrderAccepted, deserialized.MessageType);
                Assert.AreEqual(i, deserialized.LimitOrder.ClientId);
                Assert.AreEqual(i, deserialized.LimitOrder.ExchangeOrderId);
            }
        }

        [Test]
        public void ShouldSerializeServerToClientMessage()
        {
            var serializer = new ProtobufSerializer();

            for (int i = 0; i < 1000; i++)
            {
                var s = serializer.Serialize(new ServerToClientMessage {MessageType = ServerToClientMessageTypeEnum.OrderAccepted});

                Assert.IsNotNull(s);

                var d = serializer.Deserialize<ServerToClientMessage>(s);
                Assert.IsNotNull(d);
            }
        }

        [Test]
        public void SerializationPerformanceTest()
        {
            var sw = new Stopwatch();
            var serializer = new ProtobufSerializer();
            var instance = new ServerToClientMessage();
                        
            sw.Start();
            for (int i = 0; i < 5000000; i++)
                serializer.Serialize(instance);


            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            double perItem = (double)elapsed/(double)5000000;
            Console.WriteLine("Total ms: " + elapsed);
            Console.WriteLine("Per item: " + perItem);
        }

        [Test]
        public void DeSerializationPerformanceTest()
        {
            var sw = new Stopwatch();
            var serializer = new ProtobufSerializer();
            var instance = new ServerToClientMessage();
            var serialized = serializer.Serialize(instance);

            sw.Start();
            for (int i = 0; i < 5000000; i++)
                serializer.Deserialize<ServerToClientMessage>(serialized);

            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            double perItem = (double)elapsed / (double)5000000;
            Console.WriteLine("Total ms: " + elapsed);
            Console.WriteLine("Per item: " + perItem);
        }

    }
}
