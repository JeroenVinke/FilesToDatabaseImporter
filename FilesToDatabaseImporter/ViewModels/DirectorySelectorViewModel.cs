using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FilesToDatabaseImporter.Annotations;

namespace FilesToDatabaseImporter.ViewModels
{
    public class DirectorySelectorViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private bool _directorySelected;
        public bool DirectorySelected
        {
            get { return _directorySelected; }
            set
            {
                if (value.Equals(_directorySelected)) return;
                _directorySelected = value;
                OnPropertyChanged();
            }
        }

        private string _directory;
        public string Directory
        {
            get { return _directory; }
            set
            {
                if (value == _directory) return;
                _directory = value;
                OnPropertyChanged();
            }
        }

        public List<string> ExtensionArray { get; set; }

        private string _extensions;
        public string Extensions
        {
            get { return _extensions; }
            set
            {
                if (value == _extensions) return;
                _extensions = value;

                ExtensionArray = new List<string>();

                if (!string.IsNullOrEmpty(_extensions))
                {
                    var splittedExtensions = _extensions.Split(',');

                    foreach (var extension in splittedExtensions)
                    {
                        ExtensionArray.Add(extension);
                    }
                }

                OnPropertyChanged();
            }
        }

        private bool _recursive;
        public bool Recursive
        {
            get { return _recursive; }
            set
            {
                if (value.Equals(_recursive)) return;
                _recursive = value;
                OnPropertyChanged();
            }
        }

        public DirectorySelectorViewModel()
        {
            Recursive = true;
            ExtensionArray = new List<string>();
            Extensions = "html,htm";
        }

        #region IDataErrorInfo
        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                if (value.Equals(_canSave)) return;
                _canSave = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                var error = "";

                if (propertyName == "Directory")
                {
                    if (string.IsNullOrEmpty(Directory))
                    {
                        error = "Directory is mandatory";
                    }
                    else if (!System.IO.Directory.Exists(Directory))
                    {
                        error = string.Format("Directory {0} does not exist", Directory);
                    }
                }


                if (propertyName == "Extensions")
                {
                    if (!string.IsNullOrEmpty(Extensions))
                    {
                        if (Extensions.Contains("."))
                        {
                            error = "dot not allowed. Usage: html,htm";
                        }
                    }
                }

                if (Errors.ContainsKey(propertyName) && string.IsNullOrEmpty(error))
                {
                    Errors.Remove(propertyName);
                }

                if (!Errors.ContainsKey(propertyName) && !string.IsNullOrEmpty(error))
                {
                    Errors[propertyName] = error;
                }

                CanSave = !Errors.Any();

                return error;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
