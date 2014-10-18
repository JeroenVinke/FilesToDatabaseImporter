using System.Data.SqlClient;
using FilesToDatabaseImporter.Interfaces;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Helpers
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly SqlConnection _connection;
        private SqlConnectionStringBuilder _connectionStringBuilder = new SqlConnectionStringBuilder();
        private string _table;

        public DatabaseHelper()
        {
            _connection = new SqlConnection();
        }

        public void Open()
        {
            _connection.ConnectionString = _connectionStringBuilder.ToString();
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

        public SqlConnectionStringBuilder SqlConnectionStringBuilder
        {
            get { return _connectionStringBuilder; }
        }

        public void SetConnectionString(string connectionString)
        {
            _connection.ConnectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connection.ConnectionString;
        }

        public void InsertRecord(FileToDoViewModel file)
        {
            using (var sqlCommand = new SqlCommand("INSERT INTO " + _table + " (FileName, Body, CreatedDateTime) VALUES (@FileName, @Body, @CreatedDateTime)", _connection))
            {
                sqlCommand.Parameters.AddWithValue("@FileName", file.Name);
                sqlCommand.Parameters.AddWithValue("@Body", file.Content);
                sqlCommand.Parameters.AddWithValue("@CreatedDateTime", file.CreatedDate);

                sqlCommand.ExecuteNonQuery();
            }
        }

        public void SetTable(string table)
        {
            _table = table;
        }
    }
}
