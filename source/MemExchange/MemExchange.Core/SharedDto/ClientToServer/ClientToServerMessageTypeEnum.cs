using ProtoBuf;

namespace MemExchange.Core.SharedDto.ClientToServer
{
    [ProtoContract]
    public enum ClientToServerMessageTypeEnum
    {
        NotSet = 0,
        PlaceOrder = 1,
        ModifyOrder = 2,
        CancelOrder = 3,
        RequestOpenOrders = 4,
        BracketLimitOrder = 5
    }
}
