using System.Runtime.InteropServices;
using System.Windows;
using Frontend.ViewModels;

namespace Frontend.Service;

[ClassInterface(ClassInterfaceType.AutoDual)]
[ComVisible(true)]
public class JsBridge(ShipViewModel shipViewModel)
{
    public void SelectShip(string shipId)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            shipViewModel.LoadShip(shipId);
        });
    }
}