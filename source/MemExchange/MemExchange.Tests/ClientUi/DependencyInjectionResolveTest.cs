using MemExchange.Client.UI.Setup;
using MemExchange.Tests.Tools;
using NUnit.Framework;

namespace MemExchange.Tests.ClientUi
{
    [TestFixture]
    public class DependencyInjectionResolveTest
    {
        [Test]
        public void AllRegisteredInstancesShouldBeResolved()
        {
            var container = new DependencyInjection();
            container.Initialize(new Configuration());
            Assert.DoesNotThrow(() => DependencyInjectionResolveTester.CheckForPotentiallyMisconfiguredComponents(DependencyInjection.Container));
        }
    }
}
