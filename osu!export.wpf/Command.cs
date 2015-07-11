
using System;
using System.Windows.Input;

namespace osu_export.wpf
{
    public class Command : ICommand
    {
        private readonly Action<object> action;
        private readonly Predicate<object> canExecute;
        private bool isEnabled;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> action, Predicate<object> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public Command(Action<object> action)
        {
            this.action = action;
            this.canExecute = null;
            this.isEnabled = true;
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                if (this.CanExecuteChanged != null)
                {
                    this.CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute != null)
            {
                return this.canExecute(parameter);
            }
            return this.isEnabled;
        }

        public void Execute(object parameter)
        {
            this.action(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}