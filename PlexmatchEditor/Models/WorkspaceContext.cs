using System.IO;

using PlexmatchEditor.Extensions;
using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Models;

internal class WorkspaceContext(string workspacePath, IEnumerable<PlexmatchFile> plexmatchFiles)
{
    public List<PlexmatchFile> PlexmatchFiles { get; } = new(plexmatchFiles);
    private Dictionary<string, MediaFileRows> _mapped = [];

    public async ValueTask LoadAsync()
    {
        var map = new Dictionary<string, MediaFileRows>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in this.PlexmatchFiles)
        {
            await item.LoadAsync().ConfigureAwait(false);
            foreach (var row in item.Content!.Rows.OfType<PlexmatchEpisodeRow>())
            {
                var key = Path.Join(item.DirectoryRelativePath, row.FileName.Span).ToUnixPath();
                if (!map.TryGetValue(key, out var value))
                {
                    value = new();
                    map.Add(key, value);
                }
                value.AddEpisodeRow(row, item.Content!);
            }
        }
        _mapped = map;
    }

    public async ValueTask SaveAsync()
    {
        foreach (var item in this.PlexmatchFiles)
        {
            await item.SaveAsync().ConfigureAwait(false);
        }
    }

    public PlexmatchFile GetOrCreateDefaultRootPlexmatchFile()
    {
        if (this.PlexmatchFiles.Count == 0)
        {
            var newPlexmatchFile = new PlexmatchFile(new FileInfo(Path.Join(workspacePath, ".plexmatch")), workspacePath);
            this.PlexmatchFiles.Add(newPlexmatchFile);
            return newPlexmatchFile;
        }
        return this.PlexmatchFiles.First();
    }

    public IEnumerable<PlexmatchEpisodeRow[]> GetEpisodeRows(string relativePath)
    {
        foreach (var (reldir, relname) in GetPairs(relativePath.AsMemory()))
        {
            if (this.PlexmatchFiles.FirstOrDefault(x => 
                x.DirectoryRelativePath.Equals(reldir.ToString(), StringComparison.OrdinalIgnoreCase)) is { } file &&
                file.LookupEpisodeRows(relname.ToString()) is { Length: > 0 } rows)
            {
                yield return rows;
            }
        }
    }

    public PlexmatchTitleRow[] GetShowTitleRows() => this.PlexmatchFiles.SelectMany(x => x.Content!.Rows.OfType<PlexmatchTitleRow>()).ToArray();

    static IEnumerable<(ReadOnlyMemory<char>, ReadOnlyMemory<char>)> GetPairs(ReadOnlyMemory<char> relativePath)
    {
        var end = relativePath.Length;
        while (relativePath.Span[..end].LastIndexOf("/") is var index && index >= 0)
        {
            yield return (relativePath[..index], relativePath[(index + 1)..]);
            end = index;
        }
        yield return (".".AsMemory(), relativePath);
    }
}
