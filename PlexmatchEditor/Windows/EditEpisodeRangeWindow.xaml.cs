using System.Windows;

using PlexmatchEditor.Exceptions;
using PlexmatchEditor.ViewModels;

namespace PlexmatchEditor.Windows;
/// <summary>
/// EditEpisodeRangeWindow.xaml 的交互逻辑
/// </summary>
public partial class EditEpisodeRangeWindow : Window
{
    public EditEpisodeRangeWindow()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ((EpisodeRangeViewModel)this.DataContext).CreateEpisodeRange();
        }
        catch (MessageException me)
        {
            MessageBox.Show(me.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        this.DialogResult = true;
    }
}
