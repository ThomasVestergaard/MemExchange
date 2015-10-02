using System.Collections.Generic;

namespace MemExchange.Server.Clients
{
    public interface IClientRepository
    {
        /// <summary>
        /// Dictionary key is clientId.
        /// </summary>
        Dictionary<int, IClient> Clients { get; }

        IClient GetOrAddClientFromId(int clientId);
        
    }
}
