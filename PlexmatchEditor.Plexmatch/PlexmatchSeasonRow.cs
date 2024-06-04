namespace PlexmatchEditor.Plexmatch;

public record PlexmatchSeasonRow : IPlexmatchRow
{
    public const string HeaderName = "season";

    public required int Season { get; set; }

    public void Write(WriteContext writeContext) =>
        writeContext.WriteLine($"{HeaderName}: {this.Season}".AsMemory());

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line)
    {
        var trimedLine = line.Trim();
        if (trimedLine.Span.StartsWith($"{HeaderName}:", StringComparison.OrdinalIgnoreCase))
        {
            var seasonString = trimedLine[(HeaderName.Length + 1)..].Trim();
            if (int.TryParse(seasonString.Span, out var season))
            {
                return new PlexmatchSeasonRow { Season = season };
            }
        }
        return default;
    }
}
