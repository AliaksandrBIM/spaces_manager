using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System.Reflection;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Build;

partial class Build
{
	Target Compile => _ => _
		.TriggeredBy(Clean)
		.Executes(() =>
		{
			foreach (var configuration in Configurations)
			{
				if (string.IsNullOrEmpty(configuration)) throw new Exception("Configuration is not specified");

				Log.Information("Compiling {configuration} of {solution} solution", configuration, Solution);
				DotNetBuild(settings => settings
					.SetConfiguration(configuration)
					.SetDeterministic(false)
					.SetProjectFile(Solution.Path)
					.SetVerbosity(DotNetVerbosity.Quiet));
			}

			//without building Installer project will be exception kinda "Could not load assembly Wix..."
			//So wee need to build it before creating intaller

			Log.Information("Compiling {configuration} of {project} project", InstallerConfiguration, Path.GetFileName(InstallerProjectPath));
			DotNetBuild(settings => settings
				.SetConfiguration(InstallerConfiguration)
				.SetDeterministic(false)
				.SetProjectFile(InstallerProjectPath)
				.SetVerbosity(DotNetVerbosity.Quiet));

			//I also wasn't able to build the whole solution because i used to get CS2045 error from 
			//EvecaAuth project. why? I can't solve this problem

			var mainAssembly = Directory
				.GetFiles(MainProjectDirectory, "*", SearchOption.AllDirectories)
				.First(x => x.EndsWith($"{MainProjectName}.dll"));
			Version = Assembly.LoadFile(mainAssembly).GetName().Version;
		});
}