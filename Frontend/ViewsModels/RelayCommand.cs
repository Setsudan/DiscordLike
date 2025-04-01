using System.Windows.Input;

internal class RelayCommand : ICommand {
    private readonly Func<object, Task> _execute;
    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Func<object, Task> execute) {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter) {
        await _execute(parameter);
    }
}
