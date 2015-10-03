using System.Windows;
using MemExchange.Client.UI.Windows;

namespace MemExchange.Client.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = new StartupViewModel();
            var view = new Startup();
            view.DataContext = viewModel;
            view.Show();

        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {

        }
    }
}
