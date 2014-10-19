using System.Windows;

namespace FilesToDatabaseImporter.Helpers
{
    /// <summary>
    /// Helper for showing a messagebox
    /// Is mocked in unit tests
    /// </summary>
    public class MessageBoxHelper : IMessageBoxHelper
    {
        public void Show(string messageBoxText)
        {
            MessageBox.Show(messageBoxText);
        }
    }
}
