namespace PlexmatchEditor.Plexmatch;

public record PlexmatchTitleRow : IPlexmatchRow
{
    public const string TitleHeaderName = "title";
    public const string ShowHeaderName = "show";

    public required ReadOnlyMemory<char> Title { get; set; }

    public PlexmatchTitleHeaderStyle? HeaderStyle { get; set; }

    public void Write(WriteContext writeContext)
    {
        var style = this.HeaderStyle ?? writeContext.Options.PreferTitleHeaderStyle ?? PlexmatchTitleHeaderStyle.Title;
        var header = style switch
        {
            PlexmatchTitleHeaderStyle.Title => TitleHeaderName,
            PlexmatchTitleHeaderStyle.Show => ShowHeaderName,
            _ => throw new NotImplementedException()
        };
        writeContext.WriteLine($"{header}: {this.Title}".AsMemory());
    }

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line)
    {
        var trimedLine = line.Trim();
        foreach (var header in new string[] { TitleHeaderName, ShowHeaderName })
        {
            if (trimedLine.Span.StartsWith($"{header}:", StringComparison.OrdinalIgnoreCase))
            {
                return new PlexmatchTitleRow
                {
                    Title = trimedLine[(header.Length + 1)..].Trim()
                };
            }
        }
        return default;
    }
}
