using System;
using System.Collections.Generic;

namespace FilesToDatabaseImporter.Helpers
{
    public interface IDirectoryHelper
    {
        string[] GetFiles(string directory);
        DateTime GetCreatedDate(string filePath);
        string GetContent(string filePath);
        IEnumerable<string> GetDirectories(string directory);
    }
}