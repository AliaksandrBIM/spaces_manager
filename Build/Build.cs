using Nuke.Common;
namespace Build;

partial class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.Clean);
}