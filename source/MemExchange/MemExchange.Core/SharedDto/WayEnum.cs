using ProtoBuf;

namespace MemExchange.Core.SharedDto
{
    [ProtoContract]
    public enum WayEnum
    {
        Buy = 1,
        Sell = 2,
        NotSet = 3
    }
}
