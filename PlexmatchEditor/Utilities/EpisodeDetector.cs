using System.Text.RegularExpressions;

using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Utilities;

internal static partial class EpisodeDetector
{
    public static PlexmatchEpisodeRange? TryDetect(string fileName)
    {
        if (Regex_SXXEXX().Match(fileName) is { Success: true } matchSXXEXX)
        { 
            return PlexmatchEpisodeRange.TryParse(matchSXXEXX.Groups["range"].Value.AsMemory());
        }

        if (Regex_EPXX().Match(fileName) is { Success: true } matchEPXX)
        {
            var ep = int.Parse(matchEPXX.Groups["ep"].Value);
            return new PlexmatchEpisodeRange
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.EpisodeOnly,
                Start = new PlexmatchEpisodeIndex 
                { 
                    Season = 1,
                    Episode = ep 
                },
            };
        }

        return default;
    }

    [GeneratedRegex(@"[\. ](?<range>S\d{1,9}E\d{1,9})[\. ]", RegexOptions.IgnoreCase)]
    private static partial Regex Regex_SXXEXX();

    [GeneratedRegex(@"[\. ]EP(?<ep>\d{1,9})[\. ]", RegexOptions.IgnoreCase)]
    private static partial Regex Regex_EPXX();
}
