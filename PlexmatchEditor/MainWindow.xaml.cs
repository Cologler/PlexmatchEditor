using System.ComponentModel;
using System.Windows;

using Microsoft.Win32;

using PlexmatchEditor.ViewModels;
using PlexmatchEditor.Windows;

namespace PlexmatchEditor;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            var openFolder = new OpenFolderDialog();
            if (openFolder.ShowDialog() != true)
            {
                this.Close();
                return;
            }

            var workspace = new WorkspaceViewModel(openFolder.FolderName);
            workspace.ScanFilesAsync().Preserve();
            this.DataContext = workspace;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ((WorkspaceViewModel)this.DataContext).SaveAsync().Preserve();
    }

    private void EditEpisodeRangeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is MediaFileViewModel mfvm)
        {
            var ervm = new EpisodeRangeViewModel();
            if (mfvm.Episode.HasValue)
            {
                ervm.From(mfvm.Episode.Value);
            }
            var win = new EditEpisodeRangeWindow 
            { 
                DataContext = ervm,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            if (win.ShowDialog() == true)
            {
                mfvm.Episode = ervm.CreateEpisodeRange();
            }
        }
    }

    private void SetEpisodesContinuePreviousMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ((WorkspaceViewModel)this.DataContext).SetEpisodesContinuePrevious();
    }

    private async void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        var contents = await ((WorkspaceViewModel)this.DataContext).GetPlexmatchFilesContentAsync();
        var pfvm = new PreviewPlexmatchFilesViewModel 
        { 
            Files = new(contents) 
        };
        var win = new PreviewPlexmatchFilesWindow
        {
            DataContext = pfvm,
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        win.ShowDialog();
    }
}