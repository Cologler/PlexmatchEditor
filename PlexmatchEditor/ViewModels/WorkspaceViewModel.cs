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
    private readonly WorkspaceContext _workspaceContext = new(workspacePath);
    [Notify] ObservableCollection<MediaFileViewModel> _mediaFiles = [];
    [Notify] ShowTitleViewModel? _showTitle;
    [Notify] ShowYearViewModel? _showYear;

    public async ValueTask ScanFilesAsync()
    {
        var workspaceContext = this._workspaceContext;

        List<MediaFileViewModel> mediaFiles = [];
        List<PlexmatchFile> plexmatchFiles = [];

        await Task.Run(async () =>
        {
            ScanFiles(new DirectoryInfo(workspacePath));
            await workspaceContext.LoadAsync().ConfigureAwait(false);
            foreach (var item in mediaFiles)
            {
                item.LoadFromPlexmatch();
            }
        });

        this.MediaFiles = new(mediaFiles.OrderBy(x => x.Episode?.Start ??
            new PlexmatchEpisodeIndex
            {
                Season = int.MaxValue,
                Episode = int.MaxValue
            }));

        this.ShowTitle = new(workspaceContext);
        this.ShowTitle.LoadFromPlexmatch();
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
                else if (Constants.PlexmatchFileName.Equals(item.Name, StringComparison.OrdinalIgnoreCase)) // ignore case?
                {
                    workspaceContext.PlexmatchFiles.Add(new PlexmatchFile((FileInfo)item, workspacePath));
                }
                else
                {
                    mediaFiles.Add(new MediaFileViewModel((FileInfo)item, Path.GetRelativePath(workspacePath, item.FullName).ToUnixPath(), workspaceContext));
                }
            }
        }
    }

    public ValueTask SaveAsync() => this._workspaceContext!.SaveAsync();

    public async ValueTask<TextFileContentViewModel[]> GetPlexmatchFilesContentAsync()
    {
        var rv = new List<TextFileContentViewModel>();
        foreach (var item in this._workspaceContext.PlexmatchFiles)
        {
            rv.Add(await item.CreateTextFileContentAsync());
        }
        return rv.ToArray();
    }

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

    public void SetEpisodesToEpisode01()
    {
        foreach (var item in this.MediaFiles.Where(x => x.IsSelected).ToArray())
        {
            item.Episode = new PlexmatchEpisodeRange
            {
                Start = new PlexmatchEpisodeIndex
                {
                    Season = item.Episode?.Start.Season ?? 1,
                    Episode = 1
                }
            };
        }
    }
}
