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

namespace FilesToDatabaseImporter.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
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

        private bool _directorySelected;
        public bool DirectorySelected
        {
            get { return _directorySelected; }
            set
            {
                if (value.Equals(_directorySelected)) return;
                _directorySelected = value;
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



        public MainWindowViewModel()
        {
            Files = new ObservableCollection<FileToDoViewModel>();
            SqlServerViewModel = new SqlServerViewModel();
            DirectorySelectorViewModel = new DirectorySelectorViewModel();


            ImportCommand = new RelayCommand(StartImportAsync, () => SqlServerViewModel.CanSave && !Loading);
            ListFilesCommand = new RelayCommand(ListFilesAsync, () => !Loading);
        }

        private void ListFilesAsync()
        {
            if (!DirectorySelectorViewModel.CanSave)
            {
                MessageBox.Show(DirectorySelectorViewModel.Errors["Directory"]);
                return;
            }

            Files.Clear();

            Loading = true;
            Task.Run(() => ListFiles()).ContinueWith(i =>
            {
                Loading = false;
                DirectorySelected = Files.Any();
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
            var files = new FileSearch()
                .SetDirectory(DirectorySelectorViewModel.Directory)
                .SetExtension(new[] { "htm", "html" })
                .SetRecursive(DirectorySelectorViewModel.Recursive)
                .Search();

            foreach (var file in files)
            {
                var viewModel = new FileToDoViewModel(file);

                Application.Current.Dispatcher.Invoke(() => Files.Add(viewModel));
            }
        }


        public void Import()
        {
            var sqlConnection = GetSqlConnection();

            // error caught in GetSqlConnection
            if (sqlConnection == null) return;

            foreach (var file in Files)
            {
                try
                {
                    InsertRecord(file, sqlConnection);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception occurred during INSERT: " + e);
                    sqlConnection.Close();
                    return;
                }


                var file1 = file;
                Application.Current.Dispatcher.Invoke(() => file1.Done = true);
            }


            Application.Current.Dispatcher.Invoke(() => MessageBox.Show("Done"));

            sqlConnection.Close();
        }

        private void InsertRecord(FileToDoViewModel file, SqlConnection sqlConnection)
        {
            using (var sqlCommand = new SqlCommand("INSERT INTO " + SqlServerViewModel.Table + " (FileName, Body, CreatedDateTime) VALUES (@FileName, @Body, @CreatedDateTime)", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@FileName", file.Name);
                sqlCommand.Parameters.AddWithValue("@Body", file.Content);
                sqlCommand.Parameters.AddWithValue("@CreatedDateTime", file.CreatedDate);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private SqlConnection GetSqlConnection()
        {
            // connectionstringbuilder
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = SqlServerViewModel.Datasource,
                IntegratedSecurity = SqlServerViewModel.IntegratedSecurity,
                InitialCatalog = SqlServerViewModel.Database
            };

            if (!SqlServerViewModel.IntegratedSecurity)
            {
                connectionStringBuilder.UserID = SqlServerViewModel.Username;
                connectionStringBuilder.Password = SqlServerViewModel.Password;
            }



            // sqlconnection
            var databaseConnection = new SqlConnection(connectionStringBuilder.ToString());

            try
            {
                // open the connection, catch any exceptions it might throw
                databaseConnection.Open();
            }
            catch (InvalidOperationException)
            {
                Application.Current.Dispatcher.Invoke(
                    () => MessageBox.Show("Cannot open a connection without specifying a data source or server."));

                return null;
            }
            catch (SqlException)
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                        MessageBox.Show(
                            "A connection-level error occurred while opening the connection. If the Number property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the ChangePassword method for more information."));

                return null;
            }
            catch (Exception error)
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                        MessageBox.Show(
                        "Unexpected error occurred. " + error));

                return null;
            }


            return databaseConnection;
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
