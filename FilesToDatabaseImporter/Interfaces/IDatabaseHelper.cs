using System.Data.SqlClient;

namespace FilesToDatabaseImporter.Helpers
{
    public interface IDatabaseHelper
    {
        void Open();
        void Close();
        SqlConnection SqlConnection { get; }
        void SetConnectionString(string connectionString);
        string GetConnectionString();
    }
}