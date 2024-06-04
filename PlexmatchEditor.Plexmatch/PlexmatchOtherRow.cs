namespace PlexmatchEditor.Plexmatch;

public record PlexmatchOtherRow(ReadOnlyMemory<char> Content) : IPlexmatchRow
{
    public void Write(WriteContext writeContext) => writeContext.WriteLine(this.Content);

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line) => new PlexmatchOtherRow(line);
}
