using System;
using System.Windows.Input;

namespace WpfApp1
{
    public class RelayCommand : ICommand
    {
       
        readonly Func<bool> _canExecute;
        readonly Action _execute;

        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = action;
        }
        //public RelayCommand(Action action)
        //{
        //    _execute = action;
        //}


        public bool CanExecute(object parameter)
        {
            try
            {
                if (_canExecute != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"又是判断出错{ex.Message}");
                return false;

            }
        }

        public void Execute(object parameter)
        {
            _execute();
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }

            }
            remove
            {
                if ( _canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }

            }

        }
    }
}
