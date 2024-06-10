using System.Collections.ObjectModel;

using PlexmatchEditor.Models;
using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class ShowYearViewModel(WorkspaceContext workspaceContext)
{
    bool _loading;
    [Notify] private string _value = string.Empty;
    [Notify] private ObservableCollection<string> _optionsValues = [];

    private void OnValueChanged(string _, string newValue)
    {
        if (_loading)
            return;

        if (!int.TryParse(newValue, out var number))
            return;

        var yearRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Rows<PlexmatchYearRow>()).ToArray();
        if (yearRows.Length > 0)
        {
            foreach (var yearRow in yearRows)
            {
                yearRow.Year = number;
            }
        }
        else
        {
            workspaceContext.GetOrCreatePlexmatchFileForShow().InsertRow(0, new PlexmatchYearRow { Year = number });
        }
    }

    public void LoadFromPlexmatch()
    {
        var yearRows = workspaceContext.PlexmatchFiles.SelectMany(x => x.Rows<PlexmatchYearRow>()).ToArray();
        var years = yearRows.Select(x => x.Year).Distinct().ToArray();
        if (years.Length > 0)
        {
            _loading = true;
            this.OptionsValues = new(years.Select(x => x.ToString()));
            this.Value = years[0].ToString();
            _loading = false;
        }
    }
}
