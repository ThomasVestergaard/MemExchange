using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemExchange.Server.Processor.Books
{
    public interface IOrderBook
    {
        string Symbol { get; }

    }
}
