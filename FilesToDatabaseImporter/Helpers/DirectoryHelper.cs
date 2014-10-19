using System;
using System.Collections.Generic;
using System.IO;

namespace FilesToDatabaseImporter.Helpers
{
    public class DirectoryHelper : IDirectoryHelper
    {
        /// <summary>
        /// Returns all the files (with paths) of a specific directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>string array of file paths</returns>
        public string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }


        /// <summary>
        /// Returns the date when the file was created
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>DateTime of when the file was created</returns>
        public DateTime GetCreatedDate(string filePath)
        {
            return File.GetCreationTime(filePath);
        }


        /// <summary>
        /// Gets all content from a file as Text
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>content as a file as string</returns>
        public string GetContent(string filePath)
        {
            return File.ReadAllText(filePath);
        }


        /// <summary>
        /// Lists all directories of a specific directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>IEnumerable of directory paths</returns>
        public IEnumerable<string> GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }
    }
}
