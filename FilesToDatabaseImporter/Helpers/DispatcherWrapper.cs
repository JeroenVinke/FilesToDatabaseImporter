using System;
using System.Windows;
using System.Windows.Threading;
using FilesToDatabaseImporter.Interfaces;

namespace FilesToDatabaseImporter.Helpers
{
    public class DispatcherWrapper : IDispatcher
    {
        readonly Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            _dispatcher = Application.Current.Dispatcher;
        }
        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }
    }
}