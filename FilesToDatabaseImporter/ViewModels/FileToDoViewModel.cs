using FilesToDatabaseImporter.Models;

namespace FilesToDatabaseImporter.ViewModels
{
    public class FileToDoViewModel : FileViewModel
    {
        public FileToDoViewModel(File file) : base(file)
        {
        }


        private bool _done;
        public bool Done
        {
            get { return _done; }
            set
            {
                if (value.Equals(_done)) return;
                _done = value;
                OnPropertyChanged();
            }
        }
    }
}