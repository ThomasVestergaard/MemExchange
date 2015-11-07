using ProtoBuf;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    [ProtoContract]
    public enum ServerToClientMessageTypeEnum
    {
        NotSet = 1,
        LimitOrderAccepted = 2,
        LimitOrderChanged = 3,
        LimitOrderDeleted = 4,
        Message = 5,
        LimitOrderSnapshot = 6,
        Execution = 7,
        Level1 = 8,
        StopLimitOrderAccepted = 9,
        StopLimitOrderChanged = 10,
        StopLimitOrderDeleted = 11,
        StopLimitOrderSnapshot = 12
    }
}
