using ProtoBuf;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    [ProtoContract]
    public enum ServerToClientMessageTypeEnum
    {
        NotSet = 1,
        OrderAccepted = 2,
        OrderChanged = 3,
        OrderDeleted = 4,
        Message = 5,
        OrderSnapshot = 6,
        Execution = 7,
        Level1 = 8

    }
}
