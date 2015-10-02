namespace MemExchange.Core.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T inputInstance);
        T Deserialize<T>(byte[] serializedData);
    }
}
