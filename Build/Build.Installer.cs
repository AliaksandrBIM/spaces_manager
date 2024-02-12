using System.Diagnostics;
using Nuke.Common;
using Nuke.Common.Utilities;
using Serilog;
using Serilog.Events;

namespace Build;

partial class Build
{
	Target CreateInstaller => _ => _
		.TriggeredBy(Compile)
		.Executes(() =>
		{
			if (Version == null) throw new Exception("Version is not specified");

			Log.Information("Project: {Name}", MainProjectName);
			var exePattern = $"*{Solution.CI_ÑD.Installer.Name}.exe";
			var exeFile = Directory.EnumerateFiles(Solution.CI_ÑD.Installer.Directory, exePattern, SearchOption.AllDirectories).First();

			var pathArgument = MainProjectDirectory.DoubleQuoteIfNeeded();
			var versionArgument = Version.ToString().DoubleQuoteIfNeeded();
			var proc = new Process();
			proc.StartInfo.FileName = exeFile;
			proc.StartInfo.Arguments = string.Join(' ', pathArgument, versionArgument);
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.Start();

			RedirectStream(proc.StandardOutput, LogEventLevel.Information);
			RedirectStream(proc.StandardError, LogEventLevel.Error);

			proc.WaitForExit();
			if (proc.ExitCode != 0) throw new Exception($"The installer creation failed with ExitCode {proc.ExitCode}");
		});

	void RedirectStream(StreamReader reader, LogEventLevel eventLevel)
	{
		while (!reader.EndOfStream)
		{
			var value = reader.ReadLine();
			if (value is null) continue;

			var matches = StreamRegex.Matches(value);
			if (matches.Count > 0)
			{
				var parameters = matches
					.Select(match => match.Value.Substring(1, match.Value.Length - 2))
					.Cast<object>()
					.ToArray();

				var line = StreamRegex.Replace(value, match => $"{{Parameter{match.Index}}}");
				Log.Write(eventLevel, line, parameters);
			}
			else
			{
				Log.Debug(value);
			}
		}
	}
}