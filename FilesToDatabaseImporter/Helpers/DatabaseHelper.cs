using System.Data.SqlClient;

namespace FilesToDatabaseImporter.Helpers
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly SqlConnection _connection;

        public DatabaseHelper()
        {
            _connection = new SqlConnection();
        }

        public void Open()
        {
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }

        public SqlConnection SqlConnection
        {
            get { return _connection; }
        }

        public void SetConnectionString(string connectionString)
        {
            _connection.ConnectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connection.ConnectionString;
        }
    }
}
