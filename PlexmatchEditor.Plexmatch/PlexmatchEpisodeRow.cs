namespace PlexmatchEditor.Plexmatch;

public record PlexmatchEpisodeRow : IPlexmatchRow
{
    public const string LongHeaderName = "episode";
    public const string ShortHeaderName = "ep";

    public required PlexmatchEpisodeRange Episode { get; set; }

    /// <summary>
    /// Must use unix style path.
    /// </summary>
    public required ReadOnlyMemory<char> FileName { get; set; }

    public PlexmatchEpisodeHeaderStyle? HeaderStyle { get; set; }

    public void Write(WriteContext writeContext)
    {
        var style = this.HeaderStyle ?? writeContext.Options.PreferEpisodeHeaderStyle ?? PlexmatchEpisodeHeaderStyle.Long;
        var header = style switch
        {
            PlexmatchEpisodeHeaderStyle.Long => LongHeaderName,
            PlexmatchEpisodeHeaderStyle.Short => ShortHeaderName,
            _ => throw new NotImplementedException()
        };

        var episodeNumber = this.Episode.ToString(
            writeContext.Options.PreferEpisodeNumberFormat, 
            writeContext.ScopeSeason);

        writeContext.WriteLine($"{header}: {episodeNumber}: {this.FileName}".AsMemory());
    }

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line)
    {
        var trimedLine = line.Trim();
        foreach (var header in new string[] { LongHeaderName, ShortHeaderName })
        {
            if (trimedLine.Span.StartsWith($"{header}:", StringComparison.OrdinalIgnoreCase))
            {
                var content = trimedLine[(header.Length + 1)..].Trim(); // {episodeNumber}: {fileName}
                if (content.Span.IndexOf(":") is var firstColon && firstColon >= 0 &&
                    PlexmatchEpisodeRange.TryParse(content[..firstColon]) is { } episodeRange)
                {
                    return new PlexmatchEpisodeRow
                    {
                        Episode = episodeRange, 
                        FileName = content[(firstColon + 1)..].Trim() 
                    };
                }
            }
        }
        return default;
    }
}
