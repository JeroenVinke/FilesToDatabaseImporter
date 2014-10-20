using System;

namespace FilesToDatabaseImporter.Interfaces
{
    public interface IDispatcher
    {
        void Invoke(Action action);
        void BeginInvoke(Action action);
    }
}
