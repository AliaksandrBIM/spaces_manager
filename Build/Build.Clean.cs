using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace Build;

partial class Build
{
	Target Clean => _ => _
		.OnlyWhenStatic(() => IsLocalBuild)
		.Executes(() =>
		{
			CleanDirectory(ArtifactsDirectory);
			foreach (var project in Solution.AllProjects.Where(p => p.Name != "Build").ToList())
			{
				CleanDirectory(project.Directory / "bin");
				CleanDirectory(project.Directory / "obj");
			}
		});

	static void CleanDirectory(AbsolutePath path)
	{
		Log.Information("Cleaning directory: {Directory}", path);
		path.CreateOrCleanDirectory();
	}
}