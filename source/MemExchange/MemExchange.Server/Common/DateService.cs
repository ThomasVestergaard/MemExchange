using System;

namespace MemExchange.Server.Common
{
    public class DateService : IDateService
    {
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}