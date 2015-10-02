using System.Collections.Generic;

namespace MemExchange.Server.Clients
{
    public class ClientRepository : IClientRepository
    {
        public Dictionary<int, IClient> Clients { get; private set; }

        public ClientRepository()
        {
            Clients = new Dictionary<int, IClient>();
        }
        
        public IClient GetOrAddClientFromId(int clientId)
        {
            if (!Clients.ContainsKey(clientId))
                Clients.Add(clientId, new Client { ClientId = clientId});

            return Clients[clientId];
        }
    }
}