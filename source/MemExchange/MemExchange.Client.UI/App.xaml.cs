using System.Windows;
using MemExchange.Client.UI.Resources;
using MemExchange.Client.UI.Setup;
using MemExchange.Client.UI.Windows;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        public static IConfiguration Configuration { get; private set; }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = new StartupViewModel();
            var view = new Startup();

            viewModel.OkCommand = new RelayCommand(() =>
            {
                int clientId;
                if (!int.TryParse(viewModel.ClientId, out clientId))
                {
                    MessageBox.Show("Client id must be a number");
                    return;
                }

                int serverCommandPort;
                if (!int.TryParse(viewModel.CommandPort, out serverCommandPort))
                {
                    MessageBox.Show("Command port must be a number");
                    return;
                }

                int serverPublishPort;
                if (!int.TryParse(viewModel.PublishPort, out serverPublishPort))
                {
                    MessageBox.Show("Publish port must be a number");
                    return;
                }

                var config = new Configuration
                {
                    ClientId = clientId,
                    ServerAddress = viewModel.ServerAddress,
                    ServerCommandPort = serverCommandPort,
                    ServerPublishPort = serverPublishPort
                };
                StartApplication(config);
                view.Close();
            });

            viewModel.CancelCommand = new RelayCommand(() =>
            {
                view.Close();
                App_OnExit(this, null);
            });

            view.DataContext = viewModel;
            view.ShowDialog();

        }

        private void StartApplication(IConfiguration configuration)
        {
            UiDispatcher.Init(Dispatcher);
            Configuration = configuration;

            var dependencyInjection = new DependencyInjection();
            dependencyInjection.Initialize(configuration);

            DependencyInjection.Container.Resolve<IClient>().Start(configuration.ClientId, configuration.ServerAddress, configuration.ServerCommandPort, configuration.ServerPublishPort);
            

            var window = new MainWindow();
            var viewModel = new MainWindowViewModel();

            window.DataContext = viewModel;
            window.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            DependencyInjection.Container.Resolve<IClient>().Stop();
        }
    }
}
