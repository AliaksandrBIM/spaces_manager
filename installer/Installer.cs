using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WixSharp;
using File = WixSharp.File;

namespace Installer;

internal static partial class Installer
{
	const string SolutionName = "SpacesManager";
	const string ProjectGuid = "B9407ADF-08DE-4B00-9CF8-57BBC53E9B14";
	const string InstallDir = @"C:\ProgramData\Autodesk\Revit\Addins";
	const string EnecaItEmail = "eneca.bim@gmail.com";
	const string PicturePath = "Installer\\Resources\\Icons\\SpacesManager.ico";
	const string Manufacturer = "LLC Eneca";
	public static void Main(string[] args)
	{
		Logger.LogInput(args);
		var projectPath = args[0];
		Version version = new Version(args[1]);

		var project = new ManagedProject
		{
			OutDir = "output",
			OutFileName = $"{SolutionName}_{version}",
			InstallScope = InstallScope.perMachine,
			Name = SolutionName,
			GUID = new Guid(ProjectGuid),
			Platform = Platform.x64,
			UI = WUI.WixUI_ProgressOnly,
			Version = version,
			MajorUpgrade = new MajorUpgrade
			{
				AllowSameVersionUpgrades = true,
				DowngradeErrorMessage =
					$"A newer version of {SolutionName} is already installed. If you are sure you want to downgrade, remove the existing installation via Programs and Features.",
			},
			ControlPanelInfo =
			{
				Manufacturer = Manufacturer,
				ProductIcon = PicturePath,
				Contact = EnecaItEmail
			}
		};
		project.BeforeInstall += ProjectOnBeforeInstall;
		var directories = Directory.GetDirectories(projectPath, "bin/Release*", SearchOption.AllDirectories);
		if (directories.Length == 0) throw new Exception("No files were found to create an installer");

		var addinFile = Directory.GetFiles(projectPath, "*.addin").First();

		List<Dir> dirs = new List<Dir>();
		foreach (var releaseDir in directories)
		{
			var revitVersion = Tools.GetRevitVersion(releaseDir);
			var addinDir = Path.Combine(InstallDir, revitVersion);
			var appEnecaDir = Path.Combine(addinDir, "Eneca", SolutionName);

			dirs.Add(new(addinDir, new File(addinFile)));
			dirs.Add(new(appEnecaDir, new Files(@$"{releaseDir}\*.*", s => !s.EndsWithAny(false, ".pdb", ".config"))));

			Logger.LogDirsForRevitVersion(revitVersion, releaseDir, addinDir, appEnecaDir);
		}
		project.Dirs = dirs.ToArray();
		project.ResolveWildCards();
		Logger.LogEntities(project.AllFiles, version);

		project.BuildMsi();
	}

	private static void ProjectOnBeforeInstall(SetupEventArgs e)
	{
		try
		{
			DeletePreviousInnoSetupInstall(ProjectGuid);
		}
		catch (Exception)
		{
			MessageBox.Show(
				$"Failed to uninstall previous version of {SolutionName}. Please, remove the existing installation via Programs and Features.");
		}
	}
}
