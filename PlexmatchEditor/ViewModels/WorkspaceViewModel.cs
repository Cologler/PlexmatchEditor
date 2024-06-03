using System.Collections.ObjectModel;
using System.IO;

using PlexmatchEditor.Extensions;
using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

/// <summary>
/// Or call it the root directory.
/// </summary>
internal partial class WorkspaceViewModel(string workspacePath)
{
    private WorkspaceContext? _workspaceContext;
    [Notify] ObservableCollection<MediaFileViewModel> _mediaFiles = [];
    [Notify] ShowTitleViewModel? _showTitle;
    [Notify] ShowYearViewModel? _showYear;

    public async ValueTask ScanFilesAsync()
    {
        List<FileInfo> mediaFiles = [];
        List<MediaFileViewModel> mediaFileViewModels = default!;
        List<PlexmatchFile> plexmatchFiles = [];
        WorkspaceContext workspaceContext = default!;

        await Task.Run(async () =>
        {
            ScanFiles(new DirectoryInfo(workspacePath));
            workspaceContext = new WorkspaceContext(workspacePath, plexmatchFiles);
            await workspaceContext.LoadAsync().ConfigureAwait(false);
            mediaFileViewModels = mediaFiles
                .Select(x =>
                    new MediaFileViewModel(x, Path.GetRelativePath(workspacePath, x.FullName).ToUnixPath(), workspaceContext))
                .ToList();
            foreach (var item in mediaFileViewModels)
            {
                item.LoadFromPlexmatch();
            }
        });

        this._workspaceContext = workspaceContext;
        this.MediaFiles = new(mediaFileViewModels);
        this.ShowTitle = new(workspaceContext);
        this.ShowTitle.LoadFromPlexmatch(workspaceContext);
        this.ShowYear = new(workspaceContext);
        this.ShowYear.LoadFromPlexmatch();

        void ScanFiles(DirectoryInfo curDir)
        {
            foreach (var item in curDir.EnumerateFileSystemInfos())
            {
                if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    // dir
                    ScanFiles((DirectoryInfo)item);
                }
                else if (item.Name.Equals(".plexmatch", StringComparison.OrdinalIgnoreCase)) // ignore case?
                {
                    plexmatchFiles.Add(new PlexmatchFile((FileInfo)item, workspacePath));
                }
                else
                {
                    mediaFiles.Add((FileInfo)item);
                }
            }
        }
    }

    public ValueTask SaveAsync() => this._workspaceContext!.SaveAsync();

    public void SetEpisodesContinuePrevious()
    {
        foreach (var item in this.MediaFiles.Where(x => x.IsSelected).ToArray())
        {
            if (this.MediaFiles.IndexOf(item) is var index && index > 0)
            {
                var previous = this.MediaFiles[index - 1];
                if (previous.Episode is { } episodeRange)
                {
                    var s = episodeRange.End?.Season ?? episodeRange.Start.Season;
                    var e = (episodeRange.End?.Episode ?? episodeRange.Start.Episode) + 1;
                    item.Episode = new PlexmatchEpisodeRange
                    {
                        Start = new PlexmatchEpisodeIndex { Season = s, Episode = e }
                    };
                }
            }
        }
    }
}
