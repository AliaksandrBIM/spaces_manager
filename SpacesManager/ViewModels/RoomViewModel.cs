using CommunityToolkit.Mvvm.ComponentModel;

namespace Eneca.SpacesManager.ViewModels;

public partial class RoomViewModel : ObservableObject
{
    [ObservableProperty] private string _numberRoom;
    [ObservableProperty] private string _nameRoom;
    [ObservableProperty] private string _areaRoom;
    [ObservableProperty] private string _changesRoom;
    [ObservableProperty] private string _levelRoom;
}