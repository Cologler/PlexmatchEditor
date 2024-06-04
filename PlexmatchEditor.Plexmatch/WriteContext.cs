namespace PlexmatchEditor.Plexmatch;

public readonly record struct WriteContext
{
    public required IPlexmatchWriter Writer { get; init; }

    public required WriteOptions Options { get; init; }

    public int? ScopeSeason { get; init; }

    public void WriteLine(ReadOnlyMemory<char> line) => Writer.WriteLine(line);
}
