using ProtoBuf;

namespace MemExchange.Core.SharedDto.ClientToServer
{
    [ProtoContract]
    public enum ClientToServerMessageTypeEnum
    {
        NotSet = 0,
        PlaceLimitOrder = 1,
        ModifyLimitOrder = 2,
        CancelLimitOrder = 3,
        RequestOpenOrders = 4,
        PlaeMarketOrder = 5
    }
}
