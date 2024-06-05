using System.Collections.ObjectModel;
using System.IO;

using PlexmatchEditor.Plexmatch;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class TextFileContentViewModel(string filePath)
{
    [Notify] private string _displayFileName = string.Empty;
    [Notify] private ObservableCollection<Line> _lines = [];

    public ValueTask WriteFileAsync()
    {
        var text = string.Join(Constants.NewLine, this.Lines.Select(x => x.Content));
        return new(Task.Run(() => File.WriteAllText(filePath, text)));
    }

    public void RemoveSelected() => this.Lines = new(this.Lines.Where(x => !x.IsSelected));

    public partial class Line
    {
        [Notify] bool _isSelected;
        [Notify] string _content = string.Empty;
    }
}
