using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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






        // GetDatabaseHelper
        [TestMethod]
        public void GetDatabaseHelper_OK()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Verifiable();

            var viewModel = new MainWindowViewModel(null, null, databaseHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            var privateObject = new PrivateObject(viewModel);

            var result = privateObject.Invoke("GetDatabaseHelper");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetDatabaseHelper_WithSqlAuthentication()
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
            var privateObject = new PrivateObject(viewModel);

            var result = (IDatabaseHelper)privateObject.Invoke("GetDatabaseHelper");

            // Assert
            var connectionString = fakeDatabaseHelper.GetConnectionString();
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            Assert.IsTrue(connectionStringBuilder.UserID == viewModel.SqlServerViewModel.Username && connectionStringBuilder.Password == viewModel.SqlServerViewModel.Password);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void GetDatabaseHelper_InvalidOperationExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Throws(new InvalidOperationException());

            DispatcherSetup();

            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();

            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            var privateObject = new PrivateObject(viewModel);

            var result = privateObject.Invoke("GetDatabaseHelper");

            // Assert
            Assert.IsNull(result);
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
        }

        [TestMethod]
        public void GetDatabaseHelper_SqlExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
            databaseHelperMock.Setup(i => i.Open()).Throws(MakeSqlException());

            DispatcherSetup();

            var messageBoxHelperMock = new Mock<IMessageBoxHelper>();
            messageBoxHelperMock.Setup(i => i.Show(It.IsAny<string>())).Verifiable();

            var viewModel = new MainWindowViewModel(null, dispatcherMock.Object, databaseHelperMock.Object, messageBoxHelperMock.Object);
            viewModel.SqlServerViewModel.Database = "";
            viewModel.SqlServerViewModel.Datasource = "";
            viewModel.SqlServerViewModel.IntegratedSecurity = true;

            // Act
            var privateObject = new PrivateObject(viewModel);

            var result = privateObject.Invoke("GetDatabaseHelper");

            // Assert
            Assert.IsNull(result);
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
        }

        [TestMethod]
        public void GetDatabaseHelper_ExceptionDuringOpen()
        {
            // Arrange
            var databaseHelperMock = new Mock<IDatabaseHelper>();
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
            var privateObject = new PrivateObject(viewModel);

            var result = privateObject.Invoke("GetDatabaseHelper");

            // Assert
            Assert.IsNull(result);
            messageBoxHelperMock.Verify(i => i.Show(It.IsAny<string>()));
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
