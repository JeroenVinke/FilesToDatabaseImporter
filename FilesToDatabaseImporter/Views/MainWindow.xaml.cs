using System.Windows;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Views
{
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
