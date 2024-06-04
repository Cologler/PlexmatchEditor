namespace PlexmatchEditor.Plexmatch;

public interface IPlexmatchWriter
{
    void WriteLine(ReadOnlyMemory<char> line);
}
