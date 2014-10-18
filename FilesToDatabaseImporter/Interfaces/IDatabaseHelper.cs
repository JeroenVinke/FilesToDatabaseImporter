using System.Data.SqlClient;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.ViewModels;

namespace FilesToDatabaseImporter.Interfaces
{
    public interface IDatabaseHelper
    {
        void Open();
        void Close();
        SqlConnection SqlConnection { get; }
        SqlConnectionStringBuilder SqlConnectionStringBuilder { get; }
        void SetConnectionString(string connectionString);
        string GetConnectionString();
        void InsertRecord(FileToDoViewModel file);
        void SetTable(string table);
    }
}