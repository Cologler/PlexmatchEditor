using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PlexmatchEditor.Plexmatch;

public partial record struct PlexmatchEpisodeRange(PlexmatchEpisodeIndex Start, PlexmatchEpisodeIndex? End)
{
    static readonly Regex s_EpisodeRangeRegex = BuildEpisodeRangeRegex();

    public PlexmatchEpisodeNumberFormat? EpisodeNumberFormat { get; init; }

    public readonly string ToString(PlexmatchEpisodeNumberFormat? preferFormat, int? scopeSeason)
    {
        var writeFormat = this.EpisodeNumberFormat ?? preferFormat;
        var start = this.Start;
        var end = this.End ?? start;
        var season = scopeSeason ?? start.Season;

        if (!end.Season.HasValue && start.Season.HasValue)
        {
            end = end with { Season = start.Season };
        }

        // SP04
        if (writeFormat == PlexmatchEpisodeNumberFormat.SP && start.Season == 0 && end.Season == start.Season)
        {
            if (start.Episode == end.Episode)
            {
                return $"SP{start.Episode:00}";
            }
            else
            {
                return $"SP{start.Episode:00}-{end.Episode:00}";
            }
        }

        if (season == start.Season && start.Season == end.Season) // match both to ignore season field
        {
            // 01 or 01-02
            if (writeFormat == PlexmatchEpisodeNumberFormat.NumberOnly)
            {
                if (start.Episode == end.Episode)
                {
                    return $"{start.Episode:00}";
                }
                else
                {
                    return $"{start.Episode:00}-{end.Episode:00}";
                }
            }

            // E01 or E01-E02
            if (writeFormat == PlexmatchEpisodeNumberFormat.EpisodeOnly)
            {
                if (start.Episode == end.Episode)
                {
                    return $"E{start.Episode:00}";
                }
                else
                {
                    return $"E{start.Episode:00}-E{end.Episode:00}";
                }
            }
        }

        // S03E04 or S01E01-E02 or S03E12-S03E13
        return (
            start.Season == end.Season && writeFormat != PlexmatchEpisodeNumberFormat.ExplicitSeasonAndEpisode, 
            start.Episode == end.Episode) switch
        {
            (true, true)    => $"S{start.Season ?? 1:00}E{start.Episode:00}",
            (true, false)   => $"S{start.Season ?? 1:00}E{start.Episode:00}-E{end.Episode:00}",
            (false, _)      => $"S{start.Season ?? 1:00}E{start.Episode:00}-S{end.Season ?? 1:00}E{end.Episode:00}"
        };
    }

    public static PlexmatchEpisodeRange? TryParse(ReadOnlyMemory<char> section)
    {
        if (s_EpisodeRangeRegex.Match(section.Trim().ToString()) is { Success: true } match)
        {
            var startEpisode = match.Groups["StartEpisode"];
            Debug.Assert(startEpisode.Success);

            PlexmatchEpisodeNumberFormat? format = null;
            PlexmatchEpisodeIndex start = new(int.Parse(startEpisode.Value), default);
            PlexmatchEpisodeIndex? end = null;

            if (match.Groups["StartPrefix"] is { Success: true } startPrefix)
            {
                if (match.Groups["StartSeasonNumber"] is var startSeasonNumber && startSeasonNumber.Success)
                {
                    start = start with { Season = int.Parse(startSeasonNumber.Value) };
                    format = PlexmatchEpisodeNumberFormat.SeasonAndEpisode;
                }
                else if ("SP".Equals(startPrefix.Value, StringComparison.OrdinalIgnoreCase))
                {
                    start = start with { Season = 0 };
                    format = PlexmatchEpisodeNumberFormat.SP;
                }
                else if ("E".Equals(startPrefix.Value, StringComparison.OrdinalIgnoreCase))
                {
                    format = PlexmatchEpisodeNumberFormat.EpisodeOnly;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                format = PlexmatchEpisodeNumberFormat.NumberOnly;
            }

            if (match.Groups["EndEpisode"] is { Success: true } endEpisode)
            {
                end = new PlexmatchEpisodeIndex(int.Parse(endEpisode.Value), default);

                if (match.Groups["EndSeasonNumber"] is { Success: true } endSeasonNumber) 
                {
                    end = end.Value with { Season = int.Parse(endSeasonNumber.Value) };
                    format = PlexmatchEpisodeNumberFormat.ExplicitSeasonAndEpisode;
                }
            }

            return new PlexmatchEpisodeRange(start, end)
            {
                EpisodeNumberFormat = format
            };
        }

        return default;
    }

    [GeneratedRegex(
        """
        ^
        # {1,9} is a limit for int.parse
        # start
        (?:
            (?<StartPrefix>E|SP|S(?<StartSeasonNumber>\d{1,9})E)?
            (?<StartEpisode>\d{1,9})
        )
        # end
        (?:
            -
            (?<EndPrefix>E|S(?<EndSeasonNumber>\d{1,9})E)?
            (?<EndEpisode>\d{1,9})
        )?
        $
        """, 
        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)]
    private static partial Regex BuildEpisodeRangeRegex();
}
