using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Models;

internal class MediaFileRows
{
    public List<(PlexmatchEpisodeRow, PlexmatchContent)> Values { get; } = [];

    public void AddEpisodeRow(PlexmatchEpisodeRow episodeRow, PlexmatchContent content)
    {
        this.Values.Add((episodeRow, content));
    }
}
