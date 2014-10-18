using System.Data.SqlClient;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.Interfaces;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Tests
{
    public class FakeDatabaseHelper : IDatabaseHelper
    {
        private string _connectionstring;
        private string _table = "";
        private string _userId = "";
        private string _password = "";
        private SqlConnectionStringBuilder _connectionStringBuilder;

        public void Open()
        {
        }

        public void Close()
        {
        }

        public SqlConnection SqlConnection
        {
            get
            {
                return new SqlConnection();
            }
        }

        public SqlConnectionStringBuilder SqlConnectionStringBuilder
        {
            get
            {
                if (_connectionStringBuilder == null)
                {
                    _connectionStringBuilder = new SqlConnectionStringBuilder()
                    {
                        UserID = _userId,
                        Password = _password
                    };
                }

                return _connectionStringBuilder;
            }
        }

        public void SetConnectionString(string connectionString)
        {
            _connectionstring = connectionString;
        }


        public string GetConnectionString()
        {
            return _connectionStringBuilder.ToString();
        }

        public void InsertRecord(FileToDoViewModel file)
        {
        }

        public void SetTable(string table)
        {
            _table = table;
        }
    }
}