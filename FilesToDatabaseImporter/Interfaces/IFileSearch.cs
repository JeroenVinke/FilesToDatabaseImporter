using System.Collections.Generic;
using FilesToDatabaseImporter.Models;

namespace FilesToDatabaseImporter.Helpers
{
    public interface IFileSearch
    {
        List<File> Search();
        IFileSearch SetExtension(string[] extensions);
        IFileSearch SetDirectory(string directory);
        IFileSearch SetRecursive(bool recursive);
    }
}