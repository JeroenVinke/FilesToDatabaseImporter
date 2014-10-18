using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FilesToDatabaseImporter.Annotations;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.Interfaces;

namespace FilesToDatabaseImporter.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private IFileSearch _fileSearch;
        public IFileSearch FileSearch
        {
            get { return _fileSearch; }
            set { _fileSearch = value; }
        }

        private IDispatcher _dispatcher;
        public IDispatcher Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    _dispatcher = new DispatcherWrapper();
                }

                return _dispatcher;
            }
        }

        private IDatabaseHelper _databaseHelper;
        private IMessageBoxHelper _messageBoxHelper;

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (value.Equals(_loading)) return;
                _loading = value;
                OnPropertyChanged();
            }
        }

        
        // ViewModels
        public SqlServerViewModel SqlServerViewModel { get;set; }
        public DirectorySelectorViewModel DirectorySelectorViewModel { get; set; }

        public ObservableCollection<FileToDoViewModel> Files { get; set; }


        // Commands
        public ICommand ImportCommand { get; set; }
        public ICommand ListFilesCommand { get; set; }



        public MainWindowViewModel(IFileSearch fileSearch = null, IDispatcher dispatcher = null, IDatabaseHelper databaseHelper = null, IMessageBoxHelper messageBoxHelper = null)
        {
            _dispatcher = dispatcher;
            _databaseHelper = databaseHelper ?? new DatabaseHelper();
            _messageBoxHelper = messageBoxHelper ?? new MessageBoxHelper();


            Files = new ObservableCollection<FileToDoViewModel>();
            SqlServerViewModel = new SqlServerViewModel(_databaseHelper);
            DirectorySelectorViewModel = new DirectorySelectorViewModel();
            FileSearch = fileSearch ?? new FileSearch();


            ImportCommand = new RelayCommand(StartImportAsync, () => SqlServerViewModel.CanSave && !Loading);
            ListFilesCommand = new RelayCommand(ListFilesAsync, () => !Loading);
        }

        public MainWindowViewModel() : this(null)
        {   
        }

        private void ListFilesAsync()
        {
            if (!DirectorySelectorViewModel.CanSave)
            {
                _messageBoxHelper.Show(DirectorySelectorViewModel.Errors["Directory"]);
                return;
            }

            Files.Clear();

            Loading = true;
            Task.Run(() => ListFiles()).ContinueWith(i =>
            {
                Loading = false;
                DirectorySelectorViewModel.DirectorySelected = Files.Any();
            });
        }

        public void StartImportAsync()
        {
            Loading = true;
            Task.Run(() => Import()).ContinueWith(i =>
            {
                Loading = false;
            });
        }

        public void ListFiles()
        {
            var files = FileSearch
                .SetDirectory(DirectorySelectorViewModel.Directory)
                .SetExtension(new[] { "htm", "html" })
                .SetRecursive(DirectorySelectorViewModel.Recursive)
                .Search();

            foreach (var file in files)
            {
                var viewModel = new FileToDoViewModel(file);

                Dispatcher.Invoke(() => Files.Add(viewModel));
            }
        }

        public void Import()
        {
            OpenConnection();

            foreach (var file in Files)
            {
                try
                {
                    _databaseHelper.InsertRecord(file);
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() => _messageBoxHelper.Show("Exception occurred during INSERT: " + e));
                    _databaseHelper.Close();
                    return;
                }


                var file1 = file;
                Dispatcher.Invoke(() => file1.Done = true);
            }


            Dispatcher.Invoke(() => _messageBoxHelper.Show("Done"));

            _databaseHelper.Close();
        }

        public void OpenConnection()
        {
            try
            {
                // open the connection, catch any exceptions it might throw
                _databaseHelper.Open();
            }
            catch (InvalidOperationException)
            {
                Dispatcher.Invoke(
                    () => _messageBoxHelper.Show("Cannot open a connection without specifying a data source or server."));
            }
            catch (SqlException)
            {
                Dispatcher.Invoke(
                    () =>
                        _messageBoxHelper.Show(
                            "A connection-level error occurred while opening the connection. If the Number property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the ChangePassword method for more information."));
            }
            catch (Exception error)
            {
                Dispatcher.Invoke(
                    () =>
                        _messageBoxHelper.Show(
                        "Unexpected error occurred. " + error));
            }
        }

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
