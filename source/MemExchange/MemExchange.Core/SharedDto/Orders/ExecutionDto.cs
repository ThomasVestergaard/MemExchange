using System;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class ExecutionDto
    {
        [ProtoMember(1)]
        public uint ExchangeOrderId { get; set; }
        
        [ProtoMember(2)]
        public int Quantity { get; set; }

        [ProtoMember(3)]
        public string Symbol { get; set; }

        [ProtoMember(4)]
        public double Price { get; set; }

        [ProtoMember(5)]
        public DateTimeOffset ExecutionTime { get; set; }

        [ProtoMember(6)]
        public WayEnum Way { get; set; }

        public void Update(ExecutionDto other)
        {
            ExchangeOrderId = other.ExchangeOrderId;
            Quantity = other.Quantity;
            Symbol = other.Symbol;
            Price = other.Price;
            ExecutionTime = other.ExecutionTime;
            Way = other.Way;
        }

        public void Reset()
        {
            ExchangeOrderId = 0;
            Quantity = 0;
            Symbol = string.Empty;
            Price = 0;
            ExecutionTime = DateTimeOffset.MinValue;
            Way = WayEnum.NotSet;
        }
    }
}
