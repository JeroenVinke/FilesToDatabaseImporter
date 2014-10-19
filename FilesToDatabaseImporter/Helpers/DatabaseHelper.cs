using System.Data.SqlClient;
using FilesToDatabaseImporter.Interfaces;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Helpers
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly SqlConnection _connection;
        private readonly SqlConnectionStringBuilder _connectionStringBuilder = new SqlConnectionStringBuilder();
        private string _table;

        public DatabaseHelper()
        {
            _connection = new SqlConnection();
        }


        /// <summary>
        /// Builds the connectionstring and opens the SqlConnection
        /// </summary>
        public void Open()
        {
            _connection.ConnectionString = _connectionStringBuilder.ToString();
            _connection.Open();
        }


        /// <summary>
        /// Closes the SqlConnection
        /// </summary>
        public void Close()
        {
            _connection.Close();
        }


        /// <summary>
        /// Returns the SqlConnection that the DatabaseHelper uses
        /// </summary>
        public SqlConnection SqlConnection
        {
            get { return _connection; }
        }


        /// <summary>
        /// Returns the SqlConnectionStringBuilder that will be used to make the connection in Open()
        /// </summary>
        public SqlConnectionStringBuilder SqlConnectionStringBuilder
        {
            get { return _connectionStringBuilder; }
        }


        /// <summary>
        /// Sets the table which will be used in InsertRecord()
        /// </summary>
        /// <param name="table"></param>
        public void SetTable(string table)
        {
            _table = table;
        }


        /// <summary>
        /// Returns the table of the DatabaseHelper
        /// </summary>
        public string GetTable()
        {
            return _table;
        }



        /// <summary>
        /// Sets the connectionstring of the sqlconnection
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
        {
            _connection.ConnectionString = connectionString;
        }


        /// <summary>
        /// Returns the connectionstring of the SqlConnection
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return _connection.ConnectionString;
        }


        /// <summary>
        /// Inserts a record into the table
        /// </summary>
        /// <param name="file">data from FileToDoViewModel will be inserted</param>
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
    }
}
