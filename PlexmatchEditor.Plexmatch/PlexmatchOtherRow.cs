namespace PlexmatchEditor.Plexmatch;

public record PlexmatchOtherRow(ReadOnlyMemory<char> Content) : IPlexmatchRow
{
    public void Write(WriteContext writeContext) => writeContext.To.Append(this.Content).AppendLine();

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line) => new PlexmatchOtherRow(line);
}
