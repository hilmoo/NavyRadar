using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Frontend.Util;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName ?? string.Empty));
    }
}