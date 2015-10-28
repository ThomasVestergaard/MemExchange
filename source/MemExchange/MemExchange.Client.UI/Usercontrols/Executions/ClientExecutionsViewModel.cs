using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Setup;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Usercontrols.Executions
{
    public class ClientExecutionsViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ExecutionDto> Executions { get; set; }

        public ClientExecutionsViewModel(IClient client)
        {
            this.client = client;
            Executions = new ObservableCollection<ExecutionDto>();

            this.client.NewExecution += client_NewExecution;
        }

        void client_NewExecution(object sender, ExecutionDto e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                Executions.Add(e);
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
