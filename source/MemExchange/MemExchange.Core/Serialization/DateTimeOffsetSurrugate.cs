using System;
using ProtoBuf;

namespace MemExchange.Core.Serialization
{
    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        [ProtoMember(1)]
        public long Ticks
        {
            get { return Value.UtcTicks; }
            set { Value = new DateTimeOffset(value, TimeSpan.Zero); }
        }

        internal DateTimeOffset Value;

        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate surrogate)
        {
            return surrogate.Value;
        }

        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset d)
        {
            return new DateTimeOffsetSurrogate { Value = d };
        }
    }
}
