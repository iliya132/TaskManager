using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TaskManager_redesign.ViewModel.Commands
{
    public class TriggerCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _action;

        public TriggerCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }

    public class TriggerCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> Action { get; set; }
        public TriggerCommand(Action<T> action)
        {
            Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T t && parameter != null)
            {
                Action?.Invoke(t);
            }
        }
    }
}
