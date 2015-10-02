namespace MemExchange.Server.Incoming
{
    public interface IClientMessagePuller
    {
        void Start(int listenPort);
        void Stop();
    }
}
