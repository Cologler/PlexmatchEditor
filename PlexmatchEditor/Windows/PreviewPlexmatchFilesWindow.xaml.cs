using System.Windows;

using PlexmatchEditor.ViewModels;

namespace PlexmatchEditor.Windows
{
    /// <summary>
    /// PreviewPlexmatchFilesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPlexmatchFilesWindow : Window
    {
        public PreviewPlexmatchFilesWindow()
        {
            InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ActionsPanel.IsEnabled = false;

            await ((TextFileContentViewModel)this.FileNameComboBox.SelectedItem).WriteFileAsync();

            ActionsPanel.IsEnabled = true;
        }

        private async void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            ActionsPanel.IsEnabled = false;

            foreach (var file in ((PreviewPlexmatchFilesViewModel) this.DataContext).Files)
            {
                await file.WriteFileAsync();
            }

            ActionsPanel.IsEnabled = true;

            this.DialogResult = true;
        }

        private void RemoveSelectedMenuItem_Click(object sender, RoutedEventArgs e) => 
            ((TextFileContentViewModel)this.FileNameComboBox.SelectedItem).RemoveSelected();
    }
}
