namespace MemExchange.Server.Incoming
{
    public interface IIncomingMessageQueue
    {
        void Start();
        void Stop();
        void Enqueue(byte[] incomingBytes);
    }
}
