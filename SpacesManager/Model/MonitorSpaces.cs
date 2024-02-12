using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using System.Collections.ObjectModel;
using Eneca.SpacesManager.ViewModels.Utils;
using Autodesk.Revit.DB.Mechanical;
using Res = Eneca.SpacesManager.Resources.Resources;

namespace Eneca.SpacesManager.Model;
public class MonitorSpaces
{    
    public RevitLinkInstance SelectedLinkFile { get; set; }
    public ObservableCollection<RoomViewModel> CheckSpacesFromRooms(ObservableCollection<DataItemViewModel> dataItems)
    {
        ObservableCollection<RoomViewModel> roomViewModels = new();

        Document doc = RevitApi.Document;

        var oldSpaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Space>().ToList();

        var linkIntance = SelectedLinkFile;

        var linkDoc = linkIntance.GetLinkDocument();
        Transform transform = linkIntance.GetTransform();

        var nameDesignOption = RevitUtils.GetPrimaryDesignOption(linkDoc);

        var roomsLinkFile = new FilteredElementCollector(linkDoc)
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Rooms)
            .Where(e => e is Room && e.Location != null)
            .Cast<Room>()
            .ToList();

        var needRoomsLinkFile = RevitUtils.GetRoomsWithPrimaryDesignOption(roomsLinkFile, nameDesignOption);

        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToList();

        foreach (var room in needRoomsLinkFile)
        {
            foreach (var level in levels)
            {
                foreach (var item in dataItems)
                {
                    if (item.IsSelected)
                    {
                        if (room.Level.Name == item.LinkLevelName & level.Name == item.LocalLevelName)
                        {

                            XYZ locationPoint = transform.OfPoint((room.Location as LocationPoint).Point);
                            var needSpace = RevitUtils.CheckPointInSpace(oldSpaces, locationPoint);
                            if (needSpace.Count != 0)
                            {
                                List<string> changes=new();
                                var firstSpace = needSpace.First();


                                if (room.GetParameter(BuiltInParameter.ROOM_NUMBER).AsString()!=firstSpace.GetParameter(BuiltInParameter.ROOM_NUMBER).AsString())
                                {
                                    changes.Add($"{Res.Number} - {room.Number}");
                                }

                                if (firstSpace.GetParameter(BuiltInParameter.ROOM_NAME).AsString() !=room.GetParameter(BuiltInParameter.ROOM_NAME).AsString())
                                {
                                    changes.Add($"{Res.Name} -  {room.GetParameter(BuiltInParameter.ROOM_NAME).AsString()}");
                                }

                                if (firstSpace.GetParameter(Res.Area_ParName) != null & room.GetParameter(BuiltInParameter.ROOM_AREA).AsValueString() != null)
                                {
                                    var spaceAreaParameter =
                                        RevitUtils.ConvertUtilArea(firstSpace.GetParameter(Res.Area_ParName));
                                    var roomAreaParameter =
                                        RevitUtils.ConvertUtilArea(room.GetParameter(BuiltInParameter.ROOM_AREA));
                                    
                                    var difference = double.Parse(spaceAreaParameter) - double.Parse(roomAreaParameter);
                                    var roundDifference=Math.Round(difference,2);
                                    if (roundDifference!=0)
                                    {
                                        changes.Add($"{Res.AreaChanged} {roundDifference} м2");
                                    }
                                }

                                if (firstSpace.GetParameter("ADSK_Тип помещения")!=null & room.GetParameter("ADSK_Тип помещения")!=null)
                                {
                                    if ($"{firstSpace.GetParameter("ADSK_Тип помещения").AsString()}" != $"{room.GetParameter("ADSK_Тип помещения").AsString()}")
                                    {
                                        changes.Add($"ADSK_Тип помещения - {room.GetParameter("ADSK_Тип помещения").AsString()}");
                                    }
                                }

                                if (firstSpace.GetParameter("ADSK_Номер квартиры") != null & room.GetParameter("ADSK_Номер квартиры") != null)
                                {
                                    if ($"{firstSpace.GetParameter("ADSK_Номер квартиры").AsString()}" != $"{room.GetParameter("ADSK_Номер квартиры").AsString()}")
                                    {
                                        changes.Add($"ADSK_Номер квартиры - {room.GetParameter("ADSK_Номер квартиры").AsString()}");
                                    }
                                }

                                if (firstSpace.GetParameter("ADSK_Категория помещения") != null & room.GetParameter("ADSK_Категория помещения") != null)
                                {
                                    if ($"{firstSpace.GetParameter("ADSK_Категория помещения").AsString()}" != $"{room.GetParameter("ADSK_Категория помещения").AsString()}")
                                    {
                                        changes.Add($"ADSK_Категория помещения - {room.GetParameter("ADSK_Категория помещения").AsString()}");
                                    }
                                }

                                string joinedChanges = string.Join(", ", changes);

                                if (changes.Count!=0)
                                {
                                    roomViewModels.Add(new RoomViewModel
                                    {
                                        NumberRoom = room.Number,
                                        NameRoom = room.GetParameter(BuiltInParameter.ROOM_NAME).AsString(),
                                        AreaRoom = RevitUtils.ConvertUtilArea(room.GetParameter(BuiltInParameter.ROOM_AREA)), 
                                        ChangesRoom = joinedChanges, 
                                        LevelRoom = room.Level.Name
                                    });
                                }
                            }
                            
                        }
                    }
                }
            }
        }
        return roomViewModels;
    }
}