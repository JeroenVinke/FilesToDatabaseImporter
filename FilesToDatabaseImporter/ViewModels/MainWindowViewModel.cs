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
        // helpers
        private readonly IFileSearch _fileSearch;
        private readonly IDispatcher _dispatcher;
        private readonly IDatabaseHelper _databaseHelper;
        private readonly IMessageBoxHelper _messageBoxHelper;

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


        // Collections
        public ObservableCollection<FileToDoViewModel> Files { get; set; }



        // ViewModels
        public SqlServerViewModel SqlServerViewModel { get;set; }
        public DirectorySelectorViewModel DirectorySelectorViewModel { get; set; }





        // Commands
        public ICommand ImportCommand { get; set; }
        public ICommand ListFilesCommand { get; set; }



        public MainWindowViewModel(IFileSearch fileSearch = null, IDispatcher dispatcher = null, IDatabaseHelper databaseHelper = null, IMessageBoxHelper messageBoxHelper = null)
        {
            // initialize helpers
            _dispatcher = dispatcher ?? new DispatcherWrapper();
            _databaseHelper = databaseHelper ?? new DatabaseHelper();
            _messageBoxHelper = messageBoxHelper ?? new MessageBoxHelper();
            _fileSearch = fileSearch ?? new FileSearch();

            Files = new ObservableCollection<FileToDoViewModel>();

            // initialize viewmodels
            SqlServerViewModel = new SqlServerViewModel(_databaseHelper);
            DirectorySelectorViewModel = new DirectorySelectorViewModel();
            

            // initialize command
            ImportCommand = new RelayCommand(StartImportAsync, () => SqlServerViewModel.CanSave && !Loading);
            ListFilesCommand = new RelayCommand(ListFilesAsync, () => !Loading);
        }

        public MainWindowViewModel() : this(null)
        {   
        }



        /// <summary>
        /// Calls ListFiles() async and shows loading status until it finishes
        /// </summary>
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


        /// <summary>
        /// Calls Import() async and shows loading status until it finishes
        /// </summary>
        public void StartImportAsync()
        {
            Loading = true;
            Task.Run(() => Import()).ContinueWith(i =>
            {
                Loading = false;
            });
        }

        /// <summary>
        /// Uses the FileSearch helper to fill the Files collection with FileToDoViewModels
        /// </summary>
        public void ListFiles()
        {
            var files = _fileSearch
                .SetDirectory(DirectorySelectorViewModel.Directory)
                .SetExtension(DirectorySelectorViewModel.ExtensionArray.ToArray())
                .SetRecursive(DirectorySelectorViewModel.Recursive)
                .Search();

            foreach (var file in files)
            {
                var viewModel = new FileToDoViewModel(file);

                _dispatcher.Invoke(() => Files.Add(viewModel));
            }
        }


        /// <summary>
        /// Opens the database connection
        /// Inserts all files in the Files collection into the database
        /// Closes the database connection
        /// </summary>
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
                    _dispatcher.Invoke(() => _messageBoxHelper.Show("Exception occurred during INSERT: " + e));
                    _databaseHelper.Close();
                    return;
                }


                var file1 = file;
                _dispatcher.Invoke(() => file1.Done = true);
            }


            _dispatcher.Invoke(() => _messageBoxHelper.Show("Done"));

            _databaseHelper.Close();
        }


        /// <summary>
        /// Opens the SqlConnection and catches all known exceptions with a more usable error message
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                // open the connection, catch any exceptions it might throw
                _databaseHelper.Open();
            }
            catch (InvalidOperationException)
            {
                _dispatcher.Invoke(
                    () => _messageBoxHelper.Show("Cannot open a connection without specifying a data source or server."));
            }
            catch (SqlException)
            {
                _dispatcher.Invoke(
                    () =>
                        _messageBoxHelper.Show(
                            "A connection-level error occurred while opening the connection. If the Number property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the ChangePassword method for more information."));
            }
            catch (Exception error)
            {
                _dispatcher.Invoke(
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
