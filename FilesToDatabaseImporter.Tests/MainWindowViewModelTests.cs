using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.Interfaces;
using FilesToDatabaseImporter.Models;
using FilesToDatabaseImporter.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FilesToDatabaseImporter.Tests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        protected Mock<IDispatcher> dispatcherMock = new Mock<IDispatcher>();

        private void DispatcherSetup()
        {
            dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>()))
                    .Callback((Action a) => a());
            dispatcherMock.Setup(x => x.BeginInvoke(It.IsAny<Action>()))
                    .Callback((Action a) => a());
        }




        // Constructor
        [TestMethod]
        public void Constructor()
        {
            // Act
            var viewModel = new MainWindowViewModel(new FileSearch(), dispatcherMock.Object, new DatabaseHelper(),
                new MessageBoxHelper());

            var privateObject = new PrivateObject(viewModel);

            // Assert
            Assert.IsNotNull(privateObject.GetField("_databaseHelper"));
            Assert.IsNotNull(privateObject.GetField("_messageBoxHelper"));
            Assert.IsNotNull(privateObject.GetField("_dispatcher"));
            Assert.IsNotNull(privateObject.GetField("_fileSearch"));
        }

        [TestMethod]
        public void Constructor_DefaultContstructor()
        {
            // Act
            var viewModel = new MainWindowViewModel();

            // Assert
            Assert.IsNotNull(viewModel.Files);
        }






        // Dispatcher
        [TestMethod]
        public void Dispatcher_null()
        {
            // Act
            var viewModel = new MainWindowViewModel(null);

            var privateObject = new PrivateObject(viewModel);

            var dispatcher = privateObject.GetProperty("Dispatcher");

            // Assert
            Assert.IsNotNull(dispatcher);
        }

        [TestMethod]
        public void Dispatcher_NotNull()
        {
            // Act
            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object);

            var privateObject = new PrivateObject(viewModel);

            var dispatcher = privateObject.GetProperty("Dispatcher");

            // Assert
            Assert.AreEqual(dispatcherMock.Object, dispatcher);
        }







        // ListFiles
        [TestMethod]
        public void ListFiles()
        {
            // Arrange
            var files = new List<File>()
            {
                new File(),
                new File()
            };

            DispatcherSetup();

            var fileSearchMock = new Mock<IFileSearch>();
            fileSearchMock.Setup(i => i.SetDirectory(It.IsAny<string>())).Returns(fileSearchMock.Object);
            fileSearchMock.Setup(i => i.SetExtension(It.IsAny<string[]>())).Returns(fileSearchMock.Object);
            fileSearchMock.Setup(i => i.SetRecursive(It.IsAny<bool>())).Returns(fileSearchMock.Object);
            fileSearchMock.Setup(i => i.Search()).Returns(files);

            var viewModel = new MainWindowViewModel(fileSearchMock.Object, dispatcherMock.Object);
            viewModel.DirectorySelectorViewModel.Directory = "";
            viewModel.DirectorySelectorViewModel.Recursive = true;

            // Act
            viewModel.ListFiles();



            // Assert
            Assert.IsTrue(viewModel.Files.Count == files.Count);
        }









        // Import
        [TestMethod]
        public void Import_OK()
        {
            // Arrange
            DispatcherSetup();

            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.InsertRecord(It.IsAny<FileToDoViewModel>())).Verifiable();

            SetupDatabaseHelper(databaseHelperMock);


            
            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show("Done")).Verifiable();



            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.Files = new ObservableCollection<FileToDoViewModel>()
            {
                new FileToDoViewModel(new File
                {
                    Name = "test",
                    Content = "Test",
                    CreatedDate = DateTime.Now
                }),
                new FileToDoViewModel(new File
                {
                    Name = "test",
                    Content = "Test",
                    CreatedDate = DateTime.Now
                })
            };


            // Act
            viewModel.Import();



            // Assert
            messageBoxHelperMock.Verify(i => i.Show("Done"));
            databaseHelperMock.Verify(i => i.InsertRecord(It.IsAny<FileToDoViewModel>()), () => Times.Exactly(viewModel.Files.Count));
        }

        [TestMethod]
        public void Import_InsertRecordThrowsError()
        {
            // Arrange
            DispatcherSetup();

            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.InsertRecord(It.IsAny<FileToDoViewModel>())).Throws(new Exception());
            databaseHelperMock.Setup(i => i.Close()).Verifiable();

            SetupDatabaseHelper(databaseHelperMock);



            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();



            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.Files = new ObservableCollection<FileToDoViewModel>()
            {
                new FileToDoViewModel(new File
                {
                    Name = "test",
                    Content = "Test",
                    CreatedDate = DateTime.Now
                }),
                new FileToDoViewModel(new File
                {
                    Name = "test",
                    Content = "Test",
                    CreatedDate = DateTime.Now
                })
            };


            // Act
            viewModel.Import();



            // Assert
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
            databaseHelperMock.Verify(i => i.Close());
            Assert.IsFalse(viewModel.Files.Any(i => i.Done));
        }





        // OpenConnection
        [TestMethod]
        public void OpenConnection_OK()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Verifiable();
            SetupDatabaseHelper(databaseHelperMock);

            var viewModel = new MainWindowViewModel(null, null, databaseHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            viewModel.OpenConnection();

            // Assert
            databaseHelperMock.Verify(i => i.Open());
        }

        [TestMethod]
        public void OpenConnection_WithSqlAuthentication()
        {
            // Arrange
            var fakeDatabaseHelper = new FakeDatabaseHelper();

            var viewModel = new MainWindowViewModel(null, null, fakeDatabaseHelper);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = false;
            viewModel.SqlServerViewModel.Username = "test";
            viewModel.SqlServerViewModel.Password = "test";

            // Act
            viewModel.OpenConnection();

            // Assert
            var connectionString = fakeDatabaseHelper.GetConnectionString();
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            Assert.IsTrue(connectionStringBuilder.UserID == viewModel.SqlServerViewModel.Username && connectionStringBuilder.Password == viewModel.SqlServerViewModel.Password);
        }

        [TestMethod]
        public void OpenConnection_InvalidOperationExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Throws(new InvalidOperationException());
            SetupDatabaseHelper(databaseHelperMock);

            DispatcherSetup();

            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();

            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            viewModel.OpenConnection();

            // Assert
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
        }

        [TestMethod]
        public void OpenConnection_SqlExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Throws(MakeSqlException());
            SetupDatabaseHelper(databaseHelperMock);

            DispatcherSetup();

            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();

            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            viewModel.OpenConnection();

            // Assert
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
        }

        [TestMethod]
        public void OpenConnection_ExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            SetupDatabaseHelper(databaseHelperMock);

            // NullReferenceException because any exception other than InvalidOperation and SqlException should be caught...
            databaseHelperMock.Setup(i => i.Open()).Throws(new NullReferenceException());

            DispatcherSetup();

            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();

            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            viewModel.OpenConnection();

            // Assert
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
        }






        private void SetupDatabaseHelper(Mock<IDatabaseHelper> databaseHelperMock)
        {
            databaseHelperMock.SetupGet(i => i.SqlConnectionStringBuilder).Returns(new SqlConnectionStringBuilder());
        }


        // http://stackoverflow.com/questions/1386962/how-to-throw-a-sqlexceptionneed-for-mocking
        public SqlException MakeSqlException()
        {
            SqlException exception = null;
            try
            {
                var conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=1");
                conn.Open();
            }
            catch (SqlException ex)
            {
                exception = ex;
            }
            return (exception);
        }
    }
}
