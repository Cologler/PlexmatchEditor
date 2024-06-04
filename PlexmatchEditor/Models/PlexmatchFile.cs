using System.Diagnostics;
using System.IO;

using PlexmatchEditor.Extensions;
using PlexmatchEditor.Plexmatch;
using PlexmatchEditor.Plexmatch.Extensions;
using PlexmatchEditor.ViewModels;

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

    public async ValueTask<TextFileContentViewModel> CreateTextFileContentAsync()
    {
        var content = this.Content;
        var lines = await content.DumpAsLinesAsync().ConfigureAwait(false);
        var textContent = new TextFileContentViewModel(file.FullName)
        {
            FileName = Path.Join(DirectoryRelativePath, Constants.PlexmatchFileName), // windows path style is ok for display
            Lines = new(lines)
        };
        return textContent;
    }

    public async ValueTask SaveAsync() => await (await this.CreateTextFileContentAsync()).WriteFileAsync();

    public PlexmatchEpisodeRow[] LookupEpisodeRows(string relativePath)
    {
        Debug.Assert(_cachedEpisodeRow != null);

        return [.. _cachedEpisodeRow.GetValueOrDefault(relativePath, [])];
    }
}
