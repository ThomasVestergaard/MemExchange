
using System;
using System.Diagnostics;
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

            for (int i = 0; i < 1000; i++)
            {
                var s = serializer.Serialize(new ServerToClientMessage {MessageType = ServerToClientMessageTypeEnum.OrderAccepted});

                Assert.IsNotNull(s);

                var d = serializer.Deserialize<ServerToClientMessage>(s);
                var d2 = serializer.Deserialize2<ServerToClientMessage>(s);
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
