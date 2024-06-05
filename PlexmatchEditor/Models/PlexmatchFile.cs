using System.IO;

using PlexmatchEditor.Extensions;
using PlexmatchEditor.Plexmatch;
using PlexmatchEditor.Plexmatch.Extensions;
using PlexmatchEditor.ViewModels;

namespace PlexmatchEditor.Models;

internal class PlexmatchFile(FileInfo file, string workspacePath)
{
    private PlexmatchContent _content = new();
    private Dictionary<string, List<PlexmatchEpisodeRow>>? _cachedEpisodeRows = default;

    /// <summary>
    /// For workspace path, it is string.Empty.
    /// </summary>
    public string DirectoryRelativePath { get; } = GetDirectoryRelativePath(workspacePath, file.Directory!.FullName);

    static string GetDirectoryRelativePath(string workspacePath, string directoryPath)
    {
        var p = Path.GetRelativePath(workspacePath, directoryPath);
        return p == "." ? string.Empty : p.ToUnixPath();
    }

    Dictionary<string, List<PlexmatchEpisodeRow>> GetCachedEpisodeRows()
    {
        return this._cachedEpisodeRows ??= _content.Rows
            .OfType<PlexmatchEpisodeRow>()
            .GroupBy(x => x.FileName.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);
    }

    public async ValueTask LoadAsync()
    {
        var content = new PlexmatchContent();

        using var stream = file.OpenRead();
        await content.LoadFromAsync(stream).ConfigureAwait(false);

        this._content = content;
    }

    public async ValueTask<TextFileContentViewModel> CreateTextFileContentAsync()
    {
        var content = this._content;
        var lines = await content.DumpAsLinesAsync().ConfigureAwait(false);
        var textContent = new TextFileContentViewModel(file.FullName)
        {
            DisplayFileName = Path.Join(DirectoryRelativePath, Constants.PlexmatchFileName), // windows path style is ok for display
            Lines = new(lines)
        };
        return textContent;
    }

    public async ValueTask SaveAsync() => await (await this.CreateTextFileContentAsync()).WriteFileAsync();

    public PlexmatchEpisodeRow[] LookupEpisodeRows(string relativePath) 
        => [.. this.GetCachedEpisodeRows().GetValueOrDefault(relativePath, [])];

    public IEnumerable<IPlexmatchRow> Rows() => this._content.Rows;

    public IEnumerable<T> Rows<T>() where T : IPlexmatchRow => this._content.Rows.OfType<T>();

    public void AddRow(IPlexmatchRow row)
    {
        this._content.Rows.Add(row);

        if (row is PlexmatchEpisodeRow)
            this._cachedEpisodeRows = default;
    }

    public void InsertRow(int index, IPlexmatchRow row)
    {
        this._content.Rows.Insert(index, row);

        if (row is PlexmatchEpisodeRow)
            this._cachedEpisodeRows = default;
    }

    /// <summary>
    /// Get path relative to this file.
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public string GetRelativePath(string relativePath)
    {
        ThrowIfNull(relativePath);

        return Path.GetRelativePath(this.DirectoryRelativePath, relativePath).ToUnixPath();
    }
}
