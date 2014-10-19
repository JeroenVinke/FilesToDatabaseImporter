using System;
using System.Windows;
using System.Windows.Threading;
using FilesToDatabaseImporter.Interfaces;

namespace FilesToDatabaseImporter.Helpers
{
    /// <summary>
    /// Essentialy wraps the Dispatcher
    /// IDispatcher interface is mocked in unit tests so that the real dispatcher is not called
    /// </summary>
    public class DispatcherWrapper : IDispatcher
    {
        readonly Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            if (Application.Current == null) return;

            _dispatcher = Application.Current.Dispatcher;
        }

        /// <summary>
        /// Invoke an action on the dispatcher
        /// </summary>
        /// <param name="action"></param>
        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }


        /// <summary>
        /// Calls BeginInvoke on the dispatcher
        /// </summary>
        /// <param name="action"></param>
        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }
    }
}