using System.Collections.ObjectModel;

using PropertyChanged.SourceGenerator;

namespace PlexmatchEditor.ViewModels;

partial class PreviewPlexmatchFilesViewModel
{
    [Notify] private ObservableCollection<TextFileContentViewModel> _files = [];
}
