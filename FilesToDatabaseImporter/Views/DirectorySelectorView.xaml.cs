using System.Windows;
using System.Windows.Forms;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Views
{
    public partial class DirectorySelectorView
    {
        public DirectorySelectorViewModel ViewModel
        {
            get { return DataContext as DirectorySelectorViewModel; }
        }

        public DirectorySelectorView()
        {
            InitializeComponent();
        }

        private void DirectoryBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ViewModel.Directory = dialog.SelectedPath;
            }
        }
    }
}
