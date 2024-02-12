using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using Eneca.SpacesManager.ViewModels.Utils;
using EnecaUI.Controls.Eneca.UserControl;
using Res = Eneca.SpacesManager.Resources.Resources;
using Eneca.SpacesManager.Model;

namespace Eneca.SpacesManager.IExternalEvents;
public class CreateSpaces: IExternalEventHandler
{
    private readonly Document _doc = RevitApi.Document;

    public RevitLinkInstance SelectedLinkFile { get; set; }

    public ObservableCollection<DataItemViewModel> DataItemsCollection;

    public void Execute(UIApplication app)
    {
        ParameterCreator parameterCreator = new ParameterCreator();
        if (parameterCreator.CheckNeedParameter() == false)
        {
            if (parameterCreator.ConfigureSharedParameters() == false)
            {
                var createdSharedFile = parameterCreator.CreateSharedFile();
                if (createdSharedFile != null)
                {
                    parameterCreator.SetExternalDefifnition(createdSharedFile);
                    StartCommand();
                }
            }
            else
            {
                StartCommand();
            }
        }
        else
        {
            StartCommand();
        }
    }
    
    private List<Space> CreateSpaceFromRoom(List<Room> rooms, Transform transform, ObservableCollection<DataItemViewModel> dataItems)
    {
        var result = new List<Space>();
        var levels = new FilteredElementCollector(_doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToList();

        var oldSpaces = new FilteredElementCollector(_doc)
            .OfClass(typeof(SpatialElement))
            .OfCategory(BuiltInCategory.OST_MEPSpaces)
            .Cast<Space>()
            .Where(space => space.Area != 0)
            .ToList();

        List<string> needParameters= new List<string>
        {
            "ADSK_Категория помещения", 
            "ADSK_Тип помещения", 
            "ADSK_Номер квартиры", 
            "ADSK_Номер квартиры",

        };

        using (Transaction t = new(_doc, "Create spaces"))
        {
            t.Start();
            foreach (var room in rooms)
            {
                foreach (var level in levels)
                {
                    foreach(var item in dataItems) 
                    {
                        if (item.IsSelected)
                        {
                            if (room.Level.Name==item.LinkLevelName & level.Name==item.LocalLevelName)
                            {

                                XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                                var oldSpace = RevitUtils.CheckPointInSpace(oldSpaces, locationPoint);
                                if (oldSpace.Count==0)
                                {
                                    Space space = _doc.Create.NewSpace(level, new UV(locationPoint.X, locationPoint.Y));
                                    space.Name = room.GetParameter(BuiltInParameter.ROOM_NAME).AsString();
                                    space.Number = room.Number;
                                    try
                                    {
                                        space.LimitOffset = room.LimitOffset;
                                    }
                                    catch
                                    {

                                    }
                                    try
                                    {
                                        space.BaseOffset = room.BaseOffset;
                                    }
                                    catch
                                    {

                                    }
                                    try
                                    {
                                        space.UpperLimit = room.UpperLimit;
                                    }
                                    catch
                                    {

                                    }

                                    foreach (var parameter in needParameters)
                                    {
                                        if (space.GetParameter(parameter) != null &
                                            room.GetParameter(parameter) != null)
                                        {
                                            space.GetParameter(parameter).Set(room.GetParameter(parameter).AsString());
                                        }
                                    }
                                    if (space.GetParameter(Res.Area_ParName)!=null)
                                    {
                                        space.GetParameter(Res.Area_ParName).Set(room.Area);
                                    }
                                    result.Add(space);
                                    
                                }
                                else
                                {
                                    oldSpace.First().Name = room.GetParameter(BuiltInParameter.ROOM_NAME).AsString();
                                    oldSpace.First().Number = room.Number;

                                    foreach (var parameter in needParameters)
                                    {
                                        if (oldSpace.First().GetParameter(parameter) != null &
                                            room.GetParameter(parameter) != null)
                                        {
                                            oldSpace.First().GetParameter(parameter)
                                                .Set(room.GetParameter(parameter).AsString());
                                        }
                                    }

                                    if (oldSpace.First().GetParameter(Res.Area_ParName)!=null)
                                    {
                                        oldSpace.First().GetParameter(Res.Area_ParName).Set(room.Area);
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            t.Commit();
        }
        return result;
    }

    private void StartCommand()
    {
        var linkIntance = SelectedLinkFile;

        var typeLinkIntance = _doc.GetElement(linkIntance.GetTypeId());

        if (typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING) != null) 
        {
            if (typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).AsInteger() == 0)
            {
                using (Transaction t = new(_doc, "Change parameter"))
                {
                    t.Start();
                    typeLinkIntance.GetParameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).Set(1);
                    _doc.Regenerate();
                    t.Commit();
                }
            }  
        
        }
        Transform transform = linkIntance.GetTransform();

        var linkDoc = linkIntance.GetLinkDocument();

        //CopyRoomDividers(linkDoc);

        var nameDesignOption = RevitUtils.GetPrimaryDesignOption(linkDoc);

        var roomsLinkFile = new FilteredElementCollector(linkDoc)
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Rooms)
            .Cast<Room>()
            .Where(room => room.Area != 0 & room.Location!=null)
            .ToList();

        var needRoomsLinkFile = RevitUtils.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);
        var newSpaces = CreateSpaceFromRoom(needRoomsLinkFile, transform, DataItemsCollection);
        EnecaMessageBox.Show($"{newSpaces.Count} {Res.MSG_CreateSpacesP1}. {Res.MSG_CreateSpacesP2} {needRoomsLinkFile.Count}", $"{Res.MSG_Title}") ;
    }

    private void CopyRoomDividers(Document linkDoc)
    {
        var roomDividerLinkDocIds = new FilteredElementCollector(linkDoc)
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_RoomSeparationLines)
            .ToElementIds();
        var roomDividerDocIds = new FilteredElementCollector(_doc)
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_MEPSpaceSeparationLines)
            .ToElementIds();
        if (roomDividerLinkDocIds.Count != 0)
        {
            Transform transform = Transform.Identity;
            using (Transaction t = new(_doc, "Copy divider"))
            {
                t.Start();
                foreach (var dividerId in roomDividerDocIds)
                {
                    _doc.Delete(dividerId);
                }
                _doc.Regenerate();
                var copyElements = ElementTransformUtils.CopyElements(linkDoc, roomDividerLinkDocIds, _doc, transform, null);
                t.Commit();
            }
        }
    }
    public string GetName()
    {
        return "CreateSpaces";
    }
}