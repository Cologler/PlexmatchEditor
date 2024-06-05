using System.Collections.ObjectModel;
using System.IO;

using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class TextFileContentViewModel(string filePath)
{
    [Notify] private string _displayFileName = string.Empty;
    [Notify] private ObservableCollection<string> _lines = [];

    public ValueTask WriteFileAsync()
    {
        var text = string.Join(Constants.NewLine, this.Lines);
        return new(Task.Run(() => File.WriteAllText(filePath, text)));
    }
}
