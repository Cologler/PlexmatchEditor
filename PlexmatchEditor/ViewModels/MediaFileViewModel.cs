using System.IO;

using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class MediaFileViewModel(FileInfo file, string relativePath, WorkspaceContext workspaceContext)
{
    bool _loading;
    [Notify] PlexmatchEpisodeRange? _episode;
    [Notify] bool _isSelected;

    public string EpisodeText => this.Episode?.ToString(PlexmatchEpisodeNumberFormat.SeasonAndEpisode, null) ?? string.Empty;

    public string Path { get; } = relativePath;

    public void LoadFromPlexmatch()
    {
        var rows = workspaceContext.GetEpisodeRows(Path).SelectMany(x => x).ToArray();

        if (rows.Length > 0)
        {
            var row = rows[0];
            _loading = true;
            this.Episode = row.Episode;
            _loading = false;
        }
    }

    void OnEpisodeChanged(PlexmatchEpisodeRange? _, PlexmatchEpisodeRange? newValue)
    {
        if (_loading || !newValue.HasValue) return;

        if (workspaceContext.GetEpisodeRows(Path).SelectMany(x => x).ToArray() is { Length: > 0 } rows)
        {
            foreach (var row in rows)
            {
                row.Episode = newValue.Value;
            }
        }
        else
        {
            var plexmatchFile = workspaceContext.GetOrCreatePlexmatchFileForDirectory(Path, fallbackToRoot: false);
            plexmatchFile.AddRow(
                new PlexmatchEpisodeRow
                { 
                    Episode = newValue.Value,
                    FileName = plexmatchFile.GetRelativePath(this.Path).AsMemory()
                });
        }
    }
}
