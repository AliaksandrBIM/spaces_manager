using System;
using System.Text.RegularExpressions;

namespace Installer;

public static class Tools
{
    public static string GetRevitVersion(string releaseDirectory)
    {
	    string pattern = @"R(\d{2})";
	    Match match = Regex.Match(releaseDirectory, pattern);
	    if (!match.Success)
	    {
		    throw new Exception("Release Directory path is does not match the regular expression!");
	    }
	    string revitVersion = match.Groups[1].Value;
	    
		return $"20{revitVersion}";
	}
}