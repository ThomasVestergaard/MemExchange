using System;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class StopLimitOrderDto
    {
        [ProtoMember(1)]
        public string Symbol { get; set; }

        [ProtoMember(2)]
        public int Quantity { get; set; }

        [ProtoMember(3)]
        public double LimitPrice { get; set; }

        [ProtoMember(4)]
        public double TriggerPrice { get; set; }

        [ProtoMember(5)]
        public WayEnum Way { get; set; }

        [ProtoMember(6)]
        public uint ExchangeOrderId { get; set; }

        [ProtoMember(7)]
        public int ClientId { get; set; }

        public void Reeset()
        {
            Symbol = String.Empty;
            Quantity = 0;
            LimitPrice = 0;
            TriggerPrice = 0;
            Way = WayEnum.NotSet;
            ExchangeOrderId = 0;
            ClientId = 0;
        }

        public void Update(StopLimitOrderDto other)
        {
            Symbol = other.Symbol;
            Quantity = other.Quantity;
            LimitPrice = other.LimitPrice;
            TriggerPrice = other.TriggerPrice;
            Way = other.Way;
            ExchangeOrderId = other.ExchangeOrderId;
            ClientId = other.ClientId;
        }

        public bool ValidateForAdd()
        {
            if (string.IsNullOrWhiteSpace(Symbol))
                return false;

            if (Quantity <= 0)
                return false;

            if (LimitPrice <= 0)
                return false;

            if (TriggerPrice <= 0)
                return false;

            if (Way == WayEnum.NotSet)
                return false;

            if (ClientId <= 0)
                return false;

            return true;
        }

    }
}
