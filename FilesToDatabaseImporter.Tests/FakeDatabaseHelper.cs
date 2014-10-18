using System.Data.SqlClient;
using FilesToDatabaseImporter.Helpers;

namespace FilesToDatabaseImporter.Tests
{
    public class FakeDatabaseHelper : IDatabaseHelper
    {
        private string _connectionstring;

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

        public void SetConnectionString(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionstring;
        }
    }
}