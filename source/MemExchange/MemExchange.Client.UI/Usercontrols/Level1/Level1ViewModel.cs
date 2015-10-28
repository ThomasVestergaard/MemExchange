using System.Collections.ObjectModel;
using System.Windows.Input;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI.Usercontrols.Level1
{
    public class Level1ViewModel
    {
        private readonly IClient client;
        public string SymbolToAdd { get; set; }
        public ObservableCollection<SymbolViewModel> SymbolViewModels { get; set; }
        public ICommand AddSymbolCommand { get; set; }

        public Level1ViewModel(IClient client)
        {
            SymbolViewModels = new ObservableCollection<SymbolViewModel>();
            this.client = client;
            SetupCommandsAndBehaviour();
        }

        public void SetupCommandsAndBehaviour()
        {
            AddSymbolCommand = new RelayCommand(() =>
            {
                if (string.IsNullOrEmpty(SymbolToAdd))
                    return;

                var l1ViewModel = new SymbolViewModel(SymbolToAdd);
                client.Level1Updated += (sender, dto) =>
                {
                    if (dto.Symbol != SymbolToAdd)
                        return;

                    l1ViewModel.Update(dto);
                };
                SymbolViewModels.Add(l1ViewModel);
            });
        }
    }
}
