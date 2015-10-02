using System.Net;
using System.Net.Sockets;

namespace MemExchange.Tests.Tools
{
    public class TcpHelper
    {
        public static int AvailableTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
