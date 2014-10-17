using System;
using System.Collections.Generic;
using System.IO;

namespace FilesToDatabaseImporter.Helpers
{
    public class DirectoryHelper : IDirectoryHelper
    {
        public string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public DateTime GetCreatedDate(string filePath)
        {
            return File.GetCreationTime(filePath);
        }

        public string GetContent(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }
    }
}
