using System.Collections.ObjectModel;

using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class ShowTitleViewModel(WorkspaceContext workspaceContext)
{
    private bool _loading;
    [Notify] private string _value = string.Empty;
    [Notify] private ObservableCollection<string> _optionsValues = [];

    private void OnValueChanged(string _, string newValue)
    {
        if (_loading) return;

        var titleRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Rows<PlexmatchTitleRow>()).ToArray();
        if (titleRows.Length > 0)
        {
            foreach (var titleRow in titleRows)
            {
                titleRow.Title = newValue.AsMemory();
            }
        }
        else
        {
            workspaceContext.GetOrCreatePlexmatchFileForShow().InsertRow(0, new PlexmatchTitleRow { Title = newValue.AsMemory() });
        }
    }

    public void LoadFromPlexmatch()
    {
        var titleRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Rows<PlexmatchTitleRow>()).ToArray();
        var titles = titleRows.Select(x => x.Title).Select(x => x.ToString()).Distinct().ToArray();
        _loading = true;
        this.Value = titles.FirstOrDefault(string.Empty);
        this.OptionsValues = new(titles);
        _loading = false;
    }
}
