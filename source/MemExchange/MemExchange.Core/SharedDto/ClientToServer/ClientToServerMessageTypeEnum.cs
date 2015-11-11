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
        RequestOpenLimitOrders = 4,
        PlaceMarketOrder = 5,
        PlaceStopLimitOrder = 6,
        ModifyStopLimitOrder = 7,
        CancelStopLimitOrder = 8,
        RequestOpenStopLimitOrders = 9,
        DuoLimitOrderUpdate = 10
    }
}
