using System;

namespace MemExchange.Server.Common
{
    public interface IDateService
    {
        DateTimeOffset UtcNow();
    }
}
