using System.Windows.Input;

namespace NavyRadar.Frontend.Util;

public class SimpleRelayCommand(Action execute, Func<bool>? canExecute = null)
    : ICommand
{
    private readonly Action _execute = execute
                                       ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => canExecute == null || canExecute();

    public void Execute(object? parameter) => _execute();
}