using Autodesk.Revit.DB;
using System.Globalization;
using System.Windows.Data;

namespace Eneca.SpacesManager.ViewModels.Converters;
public class LinkIntanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RevitLinkInstance)
        {
            var nameLinkIntance = ((RevitLinkInstance) value).Name;
            char[] separater= {':'};
            string [] result = nameLinkIntance.Split(separater);
            if (result.ElementAt(1).Contains(".rvt"))
            {
                return result.ElementAt(1);
            }
            else
            {
                return result.ElementAt(0);
            }

        }
        return System.Windows.Data.Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 