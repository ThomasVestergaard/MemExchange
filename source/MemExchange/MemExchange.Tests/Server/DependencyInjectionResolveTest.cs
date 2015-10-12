using MemExchange.Server.Common;
using MemExchange.Tests.Tools;
using NUnit.Framework;

namespace MemExchange.Tests.Server
{
    [TestFixture]
    public class DependencyInjectionResolveTest
    {
        [Test]
        public void AllRegisteredInstancesShouldBeResolved()
        {
            var container = new DependencyInjection();
            container.Initialize();
            Assert.DoesNotThrow(() => DependencyInjectionResolveTester.CheckForPotentiallyMisconfiguredComponents(container.Container));
        }
    }
}
