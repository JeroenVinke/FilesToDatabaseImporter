using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FilesToDatabaseImporter.Annotations;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.Interfaces;

namespace FilesToDatabaseImporter.ViewModels
{
    public class SqlServerViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _datasource;
        public string Datasource
        {
            get { return _datasource; }
            set
            {
                if (value == _datasource) return;
                _datasource = value;

                DatabaseHelper.SqlConnectionStringBuilder.DataSource = _datasource;

                OnPropertyChanged();
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;

                if (!IntegratedSecurity)
                {
                    DatabaseHelper.SqlConnectionStringBuilder.UserID = _username;
                }
                else
                {
                    DatabaseHelper.SqlConnectionStringBuilder.UserID = null;
                }

                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;

                if (!IntegratedSecurity)
                {
                    DatabaseHelper.SqlConnectionStringBuilder.Password = _password;
                }
                else
                {
                    DatabaseHelper.SqlConnectionStringBuilder.Password = null;
                }

                OnPropertyChanged();
            }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set
            {
                if (value == _database) return;
                _database = value;

                DatabaseHelper.SqlConnectionStringBuilder.InitialCatalog = _database;

                OnPropertyChanged();
            }
        }

        private string _table;
        public string Table
        {
            get { return _table; }
            set
            {
                if (value == _table) return;
                _table = value;

                DatabaseHelper.SetTable(_table);

                OnPropertyChanged();
            }
        }

        private bool _integratedSecurity;
        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
            set
            {
                if (value.Equals(_integratedSecurity)) return;
                _integratedSecurity = value;

                DatabaseHelper.SqlConnectionStringBuilder.IntegratedSecurity = _integratedSecurity;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                }

                OnPropertyChanged();
            }
        }

        private IDatabaseHelper _databaseHelper;
        public IDatabaseHelper DatabaseHelper
        {
            get { return _databaseHelper; }
            set { _databaseHelper = value; }
        }

        public SqlServerViewModel(IDatabaseHelper databaseHelper = null)
        {
            _databaseHelper = databaseHelper;

            IntegratedSecurity = true;
            Database = "FilesToDatabaseImporter";
            Table = "Imports";
        }

        public SqlServerViewModel() : this(new DatabaseHelper())
        {
        }

        #region IDataErrorInfo
        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                if (value.Equals(_canSave)) return;
                _canSave = value;
                OnPropertyChanged();
            }
        }

        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();
        

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                var error = "";



                if (propertyName == "Datasource")
                {
                    if (string.IsNullOrEmpty(Datasource))
                    {
                        error = "Datasource is mandatory";
                    }
                }




                if (propertyName == "Database")
                {
                    if (string.IsNullOrEmpty(Database))
                    {
                        error = "Database is mandatory";
                    }
                }




                if (propertyName == "Username")
                {
                    if (string.IsNullOrEmpty(Username) && !IntegratedSecurity)
                    {
                        error = "Username is mandatory when SQL Authentication is used";
                    }
                }



                if (propertyName == "Password")
                {
                    if (string.IsNullOrEmpty(Password) && !IntegratedSecurity)
                    {
                        error = "Password is mandatory when SQL Authentication is used";
                    }
                }


                if (propertyName == "Table")
                {
                    if (string.IsNullOrEmpty(Table))
                    {
                        error = "Table is mandatory";
                    }
                }

                if (_errors.ContainsKey(propertyName) && string.IsNullOrEmpty(error))
                {
                    _errors.Remove(propertyName);
                }

                if (!_errors.ContainsKey(propertyName) && !string.IsNullOrEmpty(error))
                {
                    _errors[propertyName] = error;
                }

                CanSave = !_errors.Any();

                return error;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

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
