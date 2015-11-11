using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class DuoLimitOrderDto
    {
        [ProtoMember(1)]
        public LimitOrderDto LimitOrder1 { get; set; }
        [ProtoMember(2)]
        public LimitOrderDto LimitOrder2 { get; set; }
        [ProtoMember(3)]
        public int ClientId { get; set; }

        public DuoLimitOrderDto()
        {
            LimitOrder1 = new LimitOrderDto();
            LimitOrder2 = new LimitOrderDto();
        }

        public void Reset()
        {
            LimitOrder1.Reeset();
            LimitOrder2.Reeset();
        }

        public void Update(DuoLimitOrderDto other)
        {
            LimitOrder2.Update(other.LimitOrder2);
            LimitOrder1.Update(other.LimitOrder1);
        }

    }
}
