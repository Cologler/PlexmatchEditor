using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class ShowYearViewModel(WorkspaceContext workspaceContext)
{
    bool _loading;
    [Notify] private string _value = string.Empty;

    private void OnValueChanged(string _, string newValue)
    {
        if (_loading)
            return;

        if (!int.TryParse(newValue, out var number))
            return;

        var yearRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Content!.Rows.OfType<PlexmatchYearRow>()).ToArray();
        if (yearRows.Length > 0)
        {
            foreach (var yearRow in yearRows)
            {
                yearRow.Year = number;
            }
        }
        else
        {
            workspaceContext.GetOrCreateDefaultRootPlexmatchFile().Content.Rows
                .Insert(0, new PlexmatchYearRow { Year = number });
        }
    }

    public void LoadFromPlexmatch()
    {
        var yearRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Content!.Rows.OfType<PlexmatchYearRow>()).ToArray();
        var year = yearRows.Select(x => x.Year).Distinct().ToArray();
        if (year.Length > 0)
        {
            _loading = true;
            this.Value = year[0].ToString();
            _loading = false;
        }
    }
}
