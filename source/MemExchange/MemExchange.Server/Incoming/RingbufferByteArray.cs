using System;

namespace MemExchange.Server.Incoming
{
    public class RingbufferByteArray
    {
        public DateTimeOffset StartProcessTime { get; set; }
        private const int bufferSize = 512;
        public byte[] ByteBuffer { get; private set; }

        public int ContentLength { get; private set; }

        public RingbufferByteArray()
        {
            ByteBuffer = new byte[bufferSize];
            ContentLength = 0;
        }

        public void Set(byte[] newContent)
        {
            if (newContent.Length > bufferSize)
                throw new InvalidOperationException(string.Format("New buffer content exeeds internal size. New content size: {0} bytes. Internal buffer size: {1} bytes.", newContent.Length, bufferSize));

            Buffer.BlockCopy(newContent,0,ByteBuffer,0,newContent.Length);
            ContentLength = newContent.Length;
        }

        public void GetContent(ref byte[] target)
        {
            Buffer.BlockCopy(ByteBuffer, 0, target, 0, ContentLength);
        }

        public void Reset()
        {
            ContentLength = 0;
            Array.Clear(ByteBuffer, 0, bufferSize);
        }

    }
}
