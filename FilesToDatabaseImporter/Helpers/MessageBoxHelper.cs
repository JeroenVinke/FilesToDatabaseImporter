using System.Windows;

namespace FilesToDatabaseImporter.Helpers
{
    public class MessageBoxHelper : IMessageBoxHelper
    {
        public void Show(string messageBoxText)
        {
            MessageBox.Show(messageBoxText);
        }
    }
}
