using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eneca.SpacesManager.Model;
using Eneca.SpacesManager.ViewModels.Utils;
using Eneca.SpacesManager.Views;
using System.Collections.ObjectModel;
using System.Reflection;
using Autodesk.Revit.UI;
using Eneca.Analytics;
using Eneca.SpacesManager.IExternalEvents;
using EnecaUI.Controls.Eneca.UserControl;
using Res = Eneca.SpacesManager.Resources.Resources;

namespace Eneca.SpacesManager.ViewModels;
public partial class SpacesManagerViewModel : ObservableObject, IViewModel
{
    [ObservableProperty] private string _version;
    [ObservableProperty] private bool _isButtonEnabled;
    [ObservableProperty] private List<string> _currentLvlName;
    [ObservableProperty] private ObservableCollection<RevitLinkInstance> _linkFile;
    [ObservableProperty] private ObservableCollection<RoomViewModel>  _roomProperties;
    [ObservableProperty] private List<SpatialElement> _changedSpaces;
    [ObservableProperty] private ObservableCollection<DataItemViewModel>  _dataItems;
    private RevitLinkInstance _selectedRevitLinkInstance;
    private readonly ExternalEvent _createSpacesEvent;
    private readonly CreateSpaces _createSpaces;
    private readonly ExternalEvent _deleteUnallocatedSpacesEvent;
    private readonly DeleteUnallocatedSpaces _deleteUnallocatedSpaces;
    public Action<string> ShowMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SpacesManagerViewModel()
    {
        _createSpaces = new CreateSpaces();
        _createSpacesEvent = ExternalEvent.Create(_createSpaces);
        _deleteUnallocatedSpaces = new DeleteUnallocatedSpaces();
        _deleteUnallocatedSpacesEvent = ExternalEvent.Create( _deleteUnallocatedSpaces);
        RevitLinksService linksService = new();
        LinkFile=linksService.GetLinkTypeIntances();
        Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
    
    public RevitLinkInstance SelectedRevitLinkInstance
    {
        get => _selectedRevitLinkInstance;
        set
        {
            SetProperty(ref _selectedRevitLinkInstance, value);
            IsButtonEnabled = true;
        }
    }

    [RelayCommand]
    public void UpdateDataGridContent()
    {
        DataItems = new ObservableCollection<DataItemViewModel>();
        var lvlnames = new List<string>();
        var loadedLevelsViewModel = new LoadedLevelsViewModel(SelectedRevitLinkInstance);
        var levelsCurrent = loadedLevelsViewModel.GetCurrentLevels();
        var levelsLink = loadedLevelsViewModel.GetLinkLevels();

        foreach (var level in levelsCurrent)
        {
            lvlnames.Add($"{level.Name}");
        }

        CurrentLvlName = lvlnames;

        var checkList = new List<string>();
        var errorLevelsList=new List<string>();  
        foreach (var levelCur in levelsCurrent)
        {
            var matchingLevelLink = levelsLink.FirstOrDefault(levelLink =>
                Math.Round(levelCur.Elevation, 2) == Math.Round(levelLink.Elevation, 2));

            if (matchingLevelLink != null && !string.IsNullOrEmpty(levelCur.Name))
            {
                DataItems.Add(new DataItemViewModel
                {
                    IsSelected = true,
                    LinkLevelName = matchingLevelLink.Name,
                    LocalLevelName = CurrentLvlName[levelsCurrent.IndexOf(levelCur)]
                });
                checkList.Add(matchingLevelLink.Name);
            }
        }
        var unmatchedLevels = levelsLink.Where(levelLink => !checkList.Contains(levelLink.Name));
        foreach (var levelLink in unmatchedLevels)
        {
            DataItems.Add(new DataItemViewModel
            {
                IsSelected = true,
                LinkLevelName = levelLink.Name,
                LocalLevelName = ""
            });
            errorLevelsList.Add(levelLink.Name);
            checkList.Add(levelLink.Name);
        }
        string joinederrorLevelsList = string.Join(", ", errorLevelsList);
        if (errorLevelsList.Count > 0)
        {
            EnecaMessageBox.Show($"{Res.MSG_ErrorUnmatchingLevels} {joinederrorLevelsList}", Res.MSG_TitleError);
        }
    }

    [RelayCommand]
    private void MonitorSpaces()
    {
        MonitoringView newWindow = new();
        var monitoringViewModel = new MonitoringViewModel();
        newWindow.DataContext= monitoringViewModel;
        MonitorSpaces monitoringSpaces = new () {SelectedLinkFile = SelectedRevitLinkInstance};
        var checkSpacesFromRooms = monitoringSpaces.CheckSpacesFromRooms(DataItems);
        RoomProperties = checkSpacesFromRooms; // Создание ObservableCollection
        monitoringViewModel.RoomProperties = RoomProperties;
        newWindow.Show();
        AppCenter.TrackEvent($"{Res.MonitorSpaces_Button}");
    }

    [RelayCommand]
    public void CreateSpaces()
    {
        _createSpaces.DataItemsCollection = DataItems;
        _createSpaces.SelectedLinkFile = SelectedRevitLinkInstance;
        _createSpacesEvent.Raise();
        AppCenter.TrackEvent($"{Res.CreateSpaces_Button}");
    }

    [RelayCommand]
    public void DeleteSpaces()
    {
        _deleteUnallocatedSpacesEvent.Raise();
        AppCenter.TrackEvent($"{Res.DeleteSpaces_Button}");
    }

    public void OnApplicationClosing()
    {
        //throw new NotImplementedException();
    }
}