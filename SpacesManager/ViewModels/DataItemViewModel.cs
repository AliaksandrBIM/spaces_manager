using CommunityToolkit.Mvvm.ComponentModel;

namespace Eneca.SpacesManager.ViewModels;

public partial class DataItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private string _linkLevelName;
    [ObservableProperty] private string _localLevelName;
}