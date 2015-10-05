using System;
using System.Threading;
using System.Windows.Threading;

namespace MemExchange.Client.UI.Setup
{
    public class UiDispatcher
    {
        public static Dispatcher Dispatcher { get; private set; }

        public static void Init(Dispatcher uiDispatcher)
        {
            Dispatcher = uiDispatcher;
        }

        public static bool IsUiDisoatcher(Dispatcher dispatcher)
        {
            return Dispatcher == dispatcher;
        }

        public static void Reset()
        {
            Dispatcher = null;
        }
    }
}
