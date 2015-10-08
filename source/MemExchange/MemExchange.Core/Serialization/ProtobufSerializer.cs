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

        private byte[] buf { get; set; }
        private int bodySize = 0;
        private MemoryStream memStream;

        private MemoryStream deserializeStream;

        public ProtobufSerializer()
        {
            buf = new byte[bufferSize];
            memStream = new MemoryStream(buf, true);
            deserializeStream = new MemoryStream();
            
        }

        public byte[] Serialize<T>(T inputInstance)
        {
            memStream.Seek(0, SeekOrigin.Begin);
            Serializer.SerializeWithLengthPrefix(memStream, inputInstance, PrefixStyle.Fixed32);
            bodySize = (int)memStream.Position;
            
            return buf.Take(bodySize).ToArray();
        }

        public byte[] Serialize_<T>(T inputInstance)
        {
            var buffer = new byte[bufferSize];

            using (var ms = new MemoryStream(buffer, 0, buffer.Length, false))
            {
                Serializer.SerializeWithLengthPrefix(ms, inputInstance, PrefixStyle.Fixed32);
                bodySize = (int) ms.Position;
            }

            return buffer.Take(bodySize).ToArray();
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            using (var stream = new MemoryStream(serializedData, 0, serializedData.Length, false))
                return (T)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(stream, null, typeof(T), PrefixStyle.Fixed32, 0);
            
        }
    }
}