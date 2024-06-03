using System.Diagnostics;
using System.IO;

using PlexmatchEditor.Extensions;
using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Models;

internal class PlexmatchFile(FileInfo file, string workspacePath)
{
    private Dictionary<string, List<PlexmatchEpisodeRow>> _cachedEpisodeRow = [];

    public PlexmatchContent Content { get; private set; } = new();

    public string DirectoryRelativePath { get; } = Path.GetRelativePath(workspacePath, file.Directory!.FullName).ToUnixPath();

    public async ValueTask LoadAsync()
    {
        var content = new PlexmatchContent();

        using var stream = file.OpenRead();
        await content.LoadFromAsync(stream).ConfigureAwait(false);

        this._cachedEpisodeRow = content.Rows
            .OfType<PlexmatchEpisodeRow>()
            .GroupBy(x => x.FileName.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);
        this.Content = content;
    }

    public async ValueTask SaveAsync()
    {
        var content = this.Content;
        var text = await content.DumpAsync(default).ConfigureAwait(false);

        File.WriteAllText(file.FullName, text);
    }

    public PlexmatchEpisodeRow[] LookupEpisodeRows(string relativePath)
    {
        Debug.Assert(_cachedEpisodeRow != null);

        return [.. _cachedEpisodeRow.GetValueOrDefault(relativePath, [])];
    }
}
