using System.IO;

using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Models;

internal class WorkspaceContext(string workspacePath)
{
    public List<PlexmatchFile> PlexmatchFiles { get; } = [];

    /// <summary>
    /// Load all plexmatch files
    /// </summary>
    /// <returns></returns>
    public async ValueTask LoadAsync()
    {
        foreach (var item in this.PlexmatchFiles)
        {
            await item.LoadAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask SaveAsync()
    {
        foreach (var item in this.PlexmatchFiles)
        {
            await item.SaveAsync().ConfigureAwait(false);
        }
    }

    public PlexmatchFile GetOrCreatePlexmatchFileForDirectory(string relativePath, bool fallbackToRoot)
    {
        ThrowIfNull(relativePath);

        // from dir
        var reldir = GetPairs(relativePath.AsMemory()).Select(x => x.Item1.ToString()).First();
        if (reldir is not null &&
            this.PlexmatchFiles.FirstOrDefault(x => x.DirectoryRelativePath.Equals(reldir, StringComparison.OrdinalIgnoreCase)) is { } file)
        {
            return file;
        }

        // from root
        if (fallbackToRoot &&
            this.PlexmatchFiles.FirstOrDefault(x => x.DirectoryRelativePath.Equals(".", StringComparison.OrdinalIgnoreCase)) is { } rootfile)
        {
            return rootfile;
        }

        // create new
        var newPlexmatchFile = new PlexmatchFile(new FileInfo(Path.Join(workspacePath, reldir, Constants.PlexmatchFileName)), workspacePath);
        this.PlexmatchFiles.Add(newPlexmatchFile);
        return newPlexmatchFile;
    }

    public PlexmatchFile GetOrCreateDefaultRootPlexmatchFile()
    {
        if (this.PlexmatchFiles.Count == 0)
        {
            var newPlexmatchFile = new PlexmatchFile(new FileInfo(Path.Join(workspacePath, Constants.PlexmatchFileName)), workspacePath);
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

    public PlexmatchTitleRow[] GetShowTitleRows() => this.PlexmatchFiles.SelectMany(x => x.Rows<PlexmatchTitleRow>()).ToArray();

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
