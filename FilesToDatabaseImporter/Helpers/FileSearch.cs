using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = FilesToDatabaseImporter.Models.File;

namespace FilesToDatabaseImporter.Helpers
{
    public class FileSearch : IFileSearch
    {
        private readonly List<File> _files;
        private string[] _extensions;
        private string _directory;
        private bool _recursive;
        private readonly IDirectoryHelper _directoryHelper;

        public FileSearch(IDirectoryHelper directoryHelper = null)
        {
            _files = new List<File>();
            _recursive = true;
            _directoryHelper = directoryHelper ?? new DirectoryHelper();
            _extensions = new string[] {};
        }

        public List<File> Search()
        {
            if (string.IsNullOrEmpty(_directory))
            {
                throw new NullReferenceException("Directory is not set. Use SetDirectory()");
            }

            _files.Clear();

            Iterate(_directory);

            return _files;
        }

        public IFileSearch SetExtension(string[] extensions)
        {
            _extensions = extensions;

            return this;
        }

        public IFileSearch SetDirectory(string directory)
        {
            _directory = directory;

            return this;
        }

        private void Iterate(string directory)
        {
            foreach (var filePath in _directoryHelper.GetFiles(directory))
            {
                // filter extensions if any extensions are given
                if (_extensions != null && _extensions.Any())
                {
                    // skip file if extension does not match filter
                    if (!HasExtension(filePath, _extensions)) continue;
                }



                var file = new File
                {
                    Name = Path.GetFileName(filePath),
                    CreatedDate = _directoryHelper.GetCreatedDate(filePath),
                    Content = _directoryHelper.GetContent(filePath)
                };

                _files.Add(file);
            }

            if (!_recursive) return;

            foreach (var dir in _directoryHelper.GetDirectories(directory))
            {
                Iterate(dir);
            }
        }

        private bool HasExtension(string filePath, string[] extensions)
        {
            // skip if extension not in extension array
            var extension = Path.GetExtension(filePath);

            // file has no extension -> return false
            if (string.IsNullOrEmpty(extension)) return false;

            // strip dot from extension
            extension = extension.Remove(0,1);

            if (!extensions.Contains(extension)) return false;

            return true;
        }

        public IFileSearch SetRecursive(bool recursive)
        {
            _recursive = recursive;

            return this;
        }
    }
}
