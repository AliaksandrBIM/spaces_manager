using System.IO;
using Autodesk.Revit.Attributes;
using Eneca.Analytics;
using Eneca.SpacesManager.Core;
using Eneca.SpacesManager.ViewModels;
using Eneca.SpacesManager.Views;
using Nice3point.Revit.Toolkit.External;
using System.Reflection;
using Eneca.Licensing.Core.XmlLicense;
using Eneca.Licensing.Core.XmlLicense.Utilities;
using Eneca.Licensing.Core.XmlLicense.Validation;
using EnecaUI.Controls.Eneca.UserControl;
using Res = Eneca.SpacesManager.Resources.Resources;
using Microsoft.AppCenter;

namespace Eneca.SpacesManager.Commands;
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class EntryCommand : ExternalCommand
{
    private static SpacesManagerView _view;
    public override void Execute()
    {
        RevitApi.UiApplication ??= ExternalCommandData.Application; 

        string licenseDefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eneca", "License.lic"); 
        if (File.Exists(licenseDefaultPath))
        {
            using TextReader tr = new StreamReader(licenseDefaultPath);
            XmlLicense license = XmlLicense.Load(tr);

            var licenseValidationFailures = license.Validate()
                .Signature()
                .And()
                .IsActive()
                .And()
                .IsExpired()
                .And()
                .ValidateFullAccess("Spaces Manager")
                .AssertValidLicense()
                .ToList();

            if (licenseValidationFailures.Count > 0)
            {
                string message = licenseValidationFailures.ConvertFailuresToMessage();
                EnecaMessageBox.Show(message, Res.MSG_TitleError);
                return;
            }
        }
        else
        {
            EnecaMessageBox.Show(Res.MSG_Install, Res.MSG_TitleError);
            return;
        }

        #if (RELEASE)
                ConfigureAnalytics();
        #endif

        var viewModel = new SpacesManagerViewModel();
        _view = new SpacesManagerView(viewModel);
        _view.Show();
    }
    

    private void ConfigureAnalytics()
    {
        Analytics.AppCenter.AppName = (Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute)?.Title;
        Analytics.AppCenter.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        Analytics.AppCenter.UserName = ExternalCommandData.Application.Application.Username;
        Analytics.AppCenter.OpenedDocumentPath = ExternalCommandData.Application.ActiveUIDocument?.Document?.PathName ?? string.Empty;
        Analytics.AppCenter.HostProgramName = "Revit";
        Analytics.AppCenter.HostProgramVersion = new Version($"{ExternalCommandData.Application.Application.VersionNumber}.0.0.0");

        string key = "37faf12e-336c-4360-8cf2-83bb956b0afb";
        Analytics.AppCenter.ConfigureAnalytic(key);
    }
}