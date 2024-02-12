using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace Installer
{
	internal static partial class Installer
	{

		private static async void DeletePreviousInnoSetupInstall(string guid)
		{
			var uninstallPath = GetUninstallPath(guid);
			if (string.IsNullOrWhiteSpace(uninstallPath)) { return; }
			var pcInfo = new ProcessStartInfo
			{
				FileName = uninstallPath,
				Arguments = "/VERYSILENT"
			};
			var process = Process.Start(pcInfo);
			if (process == null) throw new Exception("Can't start uninstall process");
			await process.WaitForExitIncludeChildren();
		}

		private static string GetUninstallPath(string appGuid)
		{
			//https://stackoverflow.com/questions/24909108/get-installed-software-list-using-c-sharp
			//https://metanit.com/sharp/tutorial/20.3.php

			var uninstallFilePath = string.Empty;

			var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
			if (key == null) return uninstallFilePath;

			var appRoot = key
				.GetSubKeyNames()
				.FirstOrDefault(x => x.Contains(appGuid) && x.Contains("_is1"));
			if (appRoot != null)
			{
				var subKey = key.OpenSubKey(appRoot);
				if (subKey != null) uninstallFilePath = subKey.GetValue("UninstallString") as string;
			}

			return uninstallFilePath;
		}



		//inspired by https://stackoverflow.com/questions/7189117/find-all-child-processes-of-my-own-net-process-find-out-if-a-given-process-is
		private static async Task WaitForExitIncludeChildren(this Process process)
		{
			process.WaitForExit();

			uint processId = (uint)process.Id;

			// NOTE: Process Ids are reused!
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(
				"SELECT * " +
				"FROM Win32_Process " +
				"WHERE ParentProcessId=" + processId);
			ManagementObjectCollection collection = searcher.Get();
			if (collection.Count > 0)
			{
				foreach (var item in collection)
				{
					UInt32 childProcessId = (UInt32)item["ProcessId"];
					if ((int)childProcessId != Process.GetCurrentProcess().Id)
					{
						Process childProcess = Process.GetProcessById((int)childProcessId);
						await childProcess.WaitForExitIncludeChildren();
					}
				}
			}
		}
	}
}