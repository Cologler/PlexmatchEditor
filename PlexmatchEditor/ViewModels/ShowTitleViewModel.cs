using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class ShowTitleViewModel(WorkspaceContext workspaceContext)
{
    bool _loading;
    [Notify] string _value = string.Empty;

    private void OnValueChanged(string _, string newValue)
    {
        if (_loading) return;

        var titleRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Content!.Rows.OfType<PlexmatchTitleRow>()).ToArray();
        if (titleRows.Length > 0)
        {
            foreach (var titleRow in titleRows)
            {
                titleRow.Title = newValue.AsMemory();
            }
        }
        else
        {
            workspaceContext.GetOrCreateDefaultRootPlexmatchFile().Content.Rows.Add(
                new PlexmatchTitleRow { Title = newValue.AsMemory() });
        }
    }

    public void LoadFromPlexmatch(WorkspaceContext plexmatchFiles)
    {
        var titleRows = plexmatchFiles.PlexmatchFiles.SelectMany(x => x.Content!.Rows.OfType<PlexmatchTitleRow>()).ToArray();
        _loading = true;
        this.Value = titleRows.Select(x => x.Title).Distinct().FirstOrDefault().ToString();
        _loading = false;
    }
}
