using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Build;

partial class Build
{
	readonly AbsolutePath InstallerProjectPath = RootDirectory / "Installer" / "Installer.csproj";
	readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
	private string InstallerConfiguration => "Release";
	private string MainProjectName => Solution.Eneca_SpacesManager.Name;
	private string MainProjectDirectory => Solution.Eneca_SpacesManager.Directory;
	private string[] Configurations { get; set; } =
	{
		"Release R21", 
		"Release R22", 
		"Release R23",  
		"Release R24"
	};

	private Version Version { get; set; }
	[Solution(GenerateProjects = true)] private Solution Solution { get; set; }
	protected override void OnBuildInitialized()
	{
	}
}