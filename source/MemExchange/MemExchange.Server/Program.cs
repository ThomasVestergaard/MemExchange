using Topshelf;

namespace MemExchange.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Application>(s =>
                {
                    x.SetServiceName("MemExchange.Server");
                    x.SetDescription("MemExchange server");
                    x.SetDisplayName("MemExchange.Server");
                    s.ConstructUsing(n => new Application());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.StartManually();
                x.RunAsLocalService();
            });
        }
    }
}
