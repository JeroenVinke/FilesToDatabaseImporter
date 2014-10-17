using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FilesToDatabaseImporter.Annotations;
using FilesToDatabaseImporter.Models;

namespace FilesToDatabaseImporter.ViewModels
{
    public class FileViewModel : INotifyPropertyChanged
    {
        private readonly File _file;

        public FileViewModel(File file)
        {
            _file = file;
        }

        public string Name
        {
            get { return _file.Name; }
            set
            {
                if (value == _file.Name) return;
                _file.Name = value;
                OnPropertyChanged();
            }
        }
        public DateTime CreatedDate
        {
            get { return _file.CreatedDate; }
            set
            {
                if (value.Equals(_file.CreatedDate)) return;
                _file.CreatedDate = value;
                OnPropertyChanged();
            }
        }
        public string Content
        {
            get { return _file.Content; }
            set
            {
                if (value == _file.Content) return;
                _file.Content = value;
                OnPropertyChanged();
            }
        }

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
