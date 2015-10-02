using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MemExchange.Core.Serialization
{
    public class ProtobufSerializer : ISerializer
    {
        private int bufferSize = (int)Math.Pow(512, 2);
        
        public byte[] Serialize<T>(T inputInstance)
        {
            var buffer = new byte[bufferSize];
            int bodySize = 0;
            using (var memoryStream = new MemoryStream(buffer, true))
            {
                Serializer.SerializeWithLengthPrefix(memoryStream, inputInstance, PrefixStyle.Fixed32);
                bodySize = (int)memoryStream.Position;
            }

            return buffer.Take(bodySize).ToArray();
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            T deserialized;
            using (var stream = new MemoryStream(serializedData, 0, serializedData.Length, false))
                deserialized = (T)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(stream, null, typeof(T), PrefixStyle.Fixed32, 0);

            return deserialized;
        }
    }
}