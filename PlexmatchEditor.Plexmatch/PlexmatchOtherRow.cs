namespace PlexmatchEditor.Plexmatch;

public record PlexmatchOtherRow : IPlexmatchRow
{
    public required ReadOnlyMemory<char> Content { get; set; }

    public void Write(WriteContext writeContext) => writeContext.WriteLine(this.Content);

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line) => new PlexmatchOtherRow { Content = line };
}
