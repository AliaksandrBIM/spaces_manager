﻿using System.Text.RegularExpressions;

namespace Build;

partial class Build
{
	readonly Regex StreamRegex = StreamRegexGenerator();

	[GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
	private static partial Regex StreamRegexGenerator();
}