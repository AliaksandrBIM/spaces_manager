using System;
using File = WixSharp.File;

namespace Installer;

public static class Logger
{
    public static void LogEntities(File[] files, Version version)
    {
	    Console.WriteLine($"Files to be installed for version '{version}': ");
	    foreach (var projectAllFile in files)
	    {
		    Console.WriteLine($"'{projectAllFile.Name}'");
	    }
	}


    public static void LogDirsForRevitVersion(string revitVersion, string releaseDir, string addinDir, string appEnecaDir)
    {
	    Console.WriteLine(new string('-', 15));
	    Console.WriteLine($"Install directories for '{revitVersion}' Revit:");
		Console.WriteLine($"Release directory: '{releaseDir}'");
	    Console.WriteLine($"Add-in directory: '{addinDir}'");
	    Console.WriteLine($"Application directory: '{appEnecaDir}'");
	    Console.WriteLine();
	}

    public static void LogInput(string[] input)
    {
	    Console.WriteLine($"Installer input arguments:");
	    for (int i = 0; i < input.Length; i++)
	    {
		    Console.WriteLine($"{i + 1}. {input[i]}");
	    }

	    if (input.Length > 2) throw new ArgumentException("Arguments should contain project path");
	    var projectPath = input[0];
	    Console.WriteLine($"Project path: '{projectPath}'");

	    Version version = new Version(input[1]);
	    Console.WriteLine($"Version : '{version}'");
	}
}