using PlexmatchEditor.Exceptions;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

internal partial class EpisodeRangeViewModel
{
    [Notify] string _startSeason = string.Empty;
    [Notify] string _startEpisode = string.Empty;
    [Notify] string _endSeason = string.Empty;
    [Notify] string _endEpisode = string.Empty;

    public void From(PlexmatchEpisodeRange episodeRange)
    {
        this.StartSeason = episodeRange.Start.Season?.ToString() ?? string.Empty;
        this.StartEpisode = episodeRange.Start.Episode.ToString() ?? string.Empty;
        this.EndSeason = episodeRange.End?.Season?.ToString() ?? string.Empty;
        this.EndEpisode = episodeRange.End?.Episode.ToString() ?? string.Empty;
    }

    public PlexmatchEpisodeRange CreateEpisodeRange()
    {
        if (!int.TryParse(this.StartEpisode, out var se))
            throw new MessageException("Start Episode is not a integer");

        var start = new PlexmatchEpisodeIndex { Episode = se };

        if (this.StartSeason.Length > 0)
        {
            if (!int.TryParse(this.StartSeason, out var ss))
                throw new MessageException("Start Season is not a integer");

            start = start with { Season = ss };
        }

        if (this.EndEpisode.Length > 0)
        {
            if (!int.TryParse(this.EndEpisode, out var ee))
                throw new MessageException("End Episode is not a integer");

            var end = new PlexmatchEpisodeIndex { Episode = ee };

            if (this.EndSeason.Length > 0)
            {
                if (!int.TryParse(this.EndSeason, out var es))
                    throw new MessageException("End Season is not a integer");

                end = end with { Season = es };
            }

            return new PlexmatchEpisodeRange { Start = start, End = end };
        }

        return new PlexmatchEpisodeRange { Start = start };
    }
}
