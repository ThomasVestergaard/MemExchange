using System;
using System.Windows.Input;

namespace MemExchange.Client.UI.Resources
{
    public class RelayCommand : ICommand
    {
        private Action execute = null;
        private Predicate<object> canExecute = null;

        public RelayCommand(Action execute): this(execute, null)
        {
        }

        
        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute();
        }


    }
}
