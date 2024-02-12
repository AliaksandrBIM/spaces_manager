using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;

namespace Eneca.SpacesManager.Model;
public class RevitLinksService
{
    public ObservableCollection<RevitLinkInstance>  GetLinkTypeIntances()
    {
        Document doc = RevitApi.Document;
        var linkDocs = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
        ObservableCollection<RevitLinkInstance> linkInstances = new();
        
        foreach (var linkDoc in linkDocs)
        {
            var linkExternalFile = doc.GetElement(linkDoc.GetTypeId()).GetExternalFileReference().GetLinkedFileStatus();

            if (linkExternalFile == LinkedFileStatus.Loaded)
            {
                linkInstances.Add(linkDoc);
            }
        }
        return linkInstances; // Возвращаем список имен загруженных файлов
    }
}
