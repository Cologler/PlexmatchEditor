namespace PlexmatchEditor.Plexmatch;

public record struct PlexmatchEpisodeIndex(int Episode, int? Season) : IComparable<PlexmatchEpisodeIndex>
{
    public readonly int CompareTo(PlexmatchEpisodeIndex other)
    {
        return (this.Season ?? 1).CompareTo(other.Season ?? 1) is int comparedSeason && comparedSeason != 0
            ? comparedSeason 
            : this.Episode - other.Episode;
    }
}
