namespace PlexmatchEditor.Plexmatch.Extensions;

public static class PlexmatchContentExtensions
{
    public static async ValueTask<string> DumpAsTextAsync(this PlexmatchContent content, WriteOptions? options = default)
    {
        ThrowIfNull(content);

        var writer = new StringsListPlexmatchWriter();
        await content.DumpAsync(writer, options).ConfigureAwait(false);
        return string.Join(Constants.NewLine, writer.Lines);
    }

    public static async ValueTask<string[]> DumpAsLinesAsync(this PlexmatchContent content, WriteOptions? options = default)
    {
        ThrowIfNull(content);

        var writer = new StringsListPlexmatchWriter();
        await content.DumpAsync(writer, options).ConfigureAwait(false);
        return writer.Lines.Select(x => x.ToString()).ToArray();
    }

    class StringsListPlexmatchWriter : IPlexmatchWriter
    {
        public List<ReadOnlyMemory<char>> Lines { get; } = [];

        public void WriteLine(ReadOnlyMemory<char> line) => Lines.Add(line);
    }
}
