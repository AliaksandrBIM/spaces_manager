using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels.Utils;

namespace Eneca.SpacesManager.ViewModels;
public class LoadedLevelsViewModel
{
    public RevitLinkInstance SelectedLinkFile { get; set; }
    public LoadedLevelsViewModel(RevitLinkInstance selectedLinkFile)
    {
        SelectedLinkFile = selectedLinkFile;
    }

    public List<Level> GetLinkLevels()
    {
        var linkDoc = SelectedLinkFile.GetLinkDocument();
        List<string> listLevelName = RevitUtils.GetLevelsNameWithRoom(linkDoc);

        List<Level> lvlLinkFile = new FilteredElementCollector(linkDoc).OfClass(typeof(Level)).Cast<Level>().ToList();

        List<Level> sortedlvlLinkLevels = new();

        foreach (var lvl in lvlLinkFile) 
        {
            if (listLevelName.Contains(lvl.Name))
            {
                sortedlvlLinkLevels.Add(lvl);
            }
            
        }

        return sortedlvlLinkLevels;
    }
    public List<Level> GetCurrentLevels()
    {
        Document doc = RevitApi.Document;
        List<Level> lvlCurrentLevels = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Level)).Cast<Level>().ToList();

        return lvlCurrentLevels;
    }
}