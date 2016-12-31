using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AppProviderOAuth
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action action)
        {
            _execute = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
