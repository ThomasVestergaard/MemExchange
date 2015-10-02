using MemExchange.Server.Clients;
using NUnit.Framework;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class ClientRepositoryTests
    {

        [Test]
        public void ShouldAddClient()
        {
            var repo = new ClientRepository();
            var client = repo.GetOrAddClientFromId(123);

            Assert.IsNotNull(client);
            Assert.AreEqual(client.ClientId, 123);
        }

        [Test]
        public void AddAndGetShouldReturnSameInstance()
        {
            var repo = new ClientRepository();
            var firstCall = repo.GetOrAddClientFromId(123);
            var secondCall = repo.GetOrAddClientFromId(123);
            
            Assert.AreEqual(firstCall, secondCall);
        }
    }
}
