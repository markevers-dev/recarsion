using System.Windows.Input;

namespace ReCarsion
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = _ => execute();
            _canExecute = canExecute != null ? new Predicate<object>(_ => canExecute()) : null;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter != null)
                return _canExecute == null || _canExecute(parameter);
            return _canExecute == null;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null)
                _execute(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
