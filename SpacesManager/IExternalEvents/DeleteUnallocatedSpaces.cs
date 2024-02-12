using Autodesk.Revit.UI;
using Eneca.SpacesManager.Core;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using EnecaUI.Controls.Eneca.UserControl;
using Res = Eneca.SpacesManager.Resources.Resources;

namespace Eneca.SpacesManager.IExternalEvents;
public class DeleteUnallocatedSpaces : IExternalEventHandler
{
    private readonly Document _doc = RevitApi.Document;
    public void Execute(UIApplication app)
    {
        var count = 0;
        var unallocatedSpaces = new FilteredElementCollector(_doc)
            .OfClass(typeof(SpatialElement))
            .OfCategory(BuiltInCategory.OST_MEPSpaces)
            .Cast<Space>()
            .Where(space => space.Area == 0)
            .ToList();
        if (unallocatedSpaces.Count > 0)
        {
            using (Transaction t = new(_doc, "Delete unallocated spaces"))
            {
                t.Start();
                foreach (var space in unallocatedSpaces)
                {
                    _doc.Delete(space.Id);
                    count++;
                }
                t.Commit();
            }
        }
        
        EnecaMessageBox.Show($"{Res.MSG_RemoteSpaces} {count}", Res.MSG_Title);
    }

    public string GetName()
    {
        return "DeleteUnallocatedSpaces";
    }
}

