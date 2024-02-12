using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Eneca.SpacesManager.ViewModels;
public partial class MonitoringViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<RoomViewModel> _roomProperties;
    [ObservableProperty] private string _version;

    public void OnApplicationClosing()
    {
        //throw new NotImplementedException();
    }
}
