using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MemExchange.Core.Logging;
using MemExchange.Core.Serialization;
using MemExchange.Server.Clients;
using MemExchange.Server.Incoming;
using MemExchange.Server.Incoming.Logging;
using MemExchange.Server.Outgoing;
using MemExchange.Server.Processor;

namespace MemExchange.Server.Common
{
    public class DependencyInjection
    {
        public IWindsorContainer Container { get; private set; }

        public void Initialize()
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<IWindsorContainer>().Instance(Container));
            Container.Register(Component.For<ILogger>().ImplementedBy<SerilogLogger>().LifestyleSingleton());
            Container.Register(Component.For<ISerializer>().ImplementedBy<ProtobufSerializer>());
            Container.Register(Component.For<IClientMessagePuller>().ImplementedBy<ClientMessagePuller>().LifestyleSingleton());
            Container.Register(Component.For<IIncomingMessageQueue>().ImplementedBy<IncomingMessageQueue>().LifestyleSingleton());
            Container.Register(Component.For<IIncomingMessageProcessor>().ImplementedBy<IncomingMessageProcessor>().LifestyleSingleton());
            Container.Register(Component.For<IOrderKeep>().ImplementedBy<OrderKeep>().LifestyleSingleton());
            Container.Register(Component.For<IClientRepository>().ImplementedBy<ClientRepository>().LifestyleSingleton());
            Container.Register(Component.For<IOutgoingQueue>().ImplementedBy<OutgoingQueue>().LifestyleSingleton());
            Container.Register(Component.For<IMessagePublisher>().ImplementedBy<MessagePublisher>().LifestyleSingleton());
            Container.Register(Component.For<IDateService>().ImplementedBy<DateService>());
            Container.Register(Component.For<IPerformanceRecorder>().ImplementedBy<PerformanceRecorderDirectConsoleOutput>());
            
        }
    }
}
