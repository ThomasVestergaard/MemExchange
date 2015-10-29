using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MemExchange.ClientApi;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;

namespace MemExchange.Client.UI.Setup
{
    public class DependencyInjection
    {
        public static IWindsorContainer Container { get; private set; }

        public void Initialize(IConfiguration configuration)
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<IWindsorContainer>().Instance(Container));
            Container.Register(Component.For<IConfiguration>().Instance(configuration));

            Container.Register(Component.For<IClient>().ImplementedBy<ClientApi.Client>().LifestyleSingleton());
            Container.Register(Component.For<ILogger>().ImplementedBy<SerilogLogger>());
            Container.Register(Component.For<ISerializer>().ImplementedBy<ProtobufSerializer>());
            Container.Register(Component.For<IMessageConnection>().ImplementedBy<MessageConnection>().LifestyleSingleton());
            Container.Register(Component.For<IServerMessageSubscriber>().ImplementedBy<ServerMessageSubscriber>().LifestyleSingleton());
        }
    }
}
