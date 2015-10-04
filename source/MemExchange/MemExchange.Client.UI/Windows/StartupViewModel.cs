using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;

namespace MemExchange.Client.UI.Windows
{
    public class StartupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string clientId;
        public string ClientId
        {
            get { return clientId; }
            set
            {
                if (value == clientId)
                    return;

                clientId = value;
                OnPropertyChanged();
            }
        }

        private string serverAddress;
        public string ServerAddress
        {
            get { return serverAddress; }
            set
            {
                if (ServerAddress == value)
                    return;

                serverAddress = value;
                OnPropertyChanged();
            }
        }

        private string commandPort;
        public string CommandPort
        {
            get { return commandPort; }
            set
            {
                if (CommandPort == value)
                    return;

                commandPort = value;
                OnPropertyChanged();
            }
        }

        private string publishPort;
        public string PublishPort
        {
            get { return publishPort; }
            set
            {
                if (value == publishPort)
                    return;

                publishPort = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public StartupViewModel()
        {
            serverAddress = "localhost";
            publishPort = "9193";
            commandPort = "9192";

            Random rnd = new Random();
            clientId = rnd.Next(1, 100).ToString();
        }
        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
