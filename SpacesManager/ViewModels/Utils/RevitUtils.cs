using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using System.IO;

namespace Eneca.SpacesManager.ViewModels.Utils;
public static class RevitUtils
{
    /// <summary>
    /// Получает имя основного варианта проектирования из заданного документа.
    /// </summary>
    /// <param name="linkDoc">Документ Revit, из которого нужно получить основной вариант проектирования.</param>
    /// <returns>Имя основного варианта проектирования или null, если основной вариант не найден.</returns>
    public static string GetPrimaryDesignOption(Document doc) 
    {
        var linkDesignOptions = new FilteredElementCollector(doc)
            .OfClass(typeof(DesignOption))
            .Cast<DesignOption>()
            .ToList();

        string designOptionName = null;
        foreach (var designOption in linkDesignOptions)
        {
            if(designOption.IsPrimary==true)
            {
                designOptionName=designOption.Name;
            }
        }
        return designOptionName;
    }   

    public static List<Room> GetRoomsWithPrimaryDesignOption(List<Room> rooms, string nameDesignOption) 
    {
        List<Room> needRooms=new();
        foreach (var room in rooms)
        {
            if (room.DesignOption == null)
            {
                needRooms.Add(room);
            }
            else
            {
                if (room.DesignOption.Name == nameDesignOption)
                {
                    needRooms.Add(room);
                }
            }
        }
        return needRooms;
    }   

    public static List<string> GetLevelsNameWithRoom(Document doc)
    {
        List<string> levelsName = new();
        var roomsLinkFile = new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Rooms)
            .Where(e => e is Room && e.Location != null)
            .Cast<Room>()
            .ToList();

        foreach (var room in roomsLinkFile) 
        { 
            levelsName.Add(room.Level.Name);
        }
        List<string> uniqueListLevelsName = levelsName.Distinct().ToList();

        return uniqueListLevelsName;
    }

    public static List<Space> CheckPointInSpace(List<Space> oldSpaces, XYZ point)
    {
        List<Space> checkList = new();
        if (oldSpaces.Count()!= 0)
        {
            foreach (Space space in oldSpaces)
            {
                if (space.IsPointInSpace(point))
                {
                    checkList.Add(space);

                }
            }
        }
        return checkList;
    }

    public static void SetExternalDefifnition(Document document)
    {
        FileInfo filePath = new FileInfo("R:/2 - Библиотека/Revit 2021/1 - Общие ресурсы/Файлы общих параметров (ФОП)/ФОП [ENECA].txt");
        
        document.Application.SharedParametersFilename = filePath.FullName;

        var category = Category.GetCategory(document, BuiltInCategory.OST_MEPSpaces);
        var categoryset = document.Application.Create.NewCategorySet();
        categoryset.Insert(category);
        var file = document.Application.OpenSharedParameterFile();
        var definition = file.Groups.get_Item("ADSK 10 Размеры").Definitions.get_Item("ADSK_Размер_Площадь");
        var newTypeBinding = document.Application.Create.NewInstanceBinding(categoryset);
        document.ParameterBindings.Insert(definition, newTypeBinding);
    }



    public static string ConvertUtilArea(Parameter parameter)
    {
        var result =Math.Round(UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), UnitTypeId.CubicMeters), 1).ToString();
        return result;
    }
}
