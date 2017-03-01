using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
    public class AuthorTest : IDisposable
    {
        public AuthorTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_AuthorsEmptyAtFirst()
        {
            //Arrange, Act
            int result = Author.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToAuthorObject()
        {
            //Arrange
            Author testAuthor = new Author("Britton");
            testAuthor.Save();

            //Act
            Author savedAuthor = Author.GetAll()[0];

            int result = savedAuthor.GetId();
            int testId = testAuthor.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        public void Dispose()
        {
            Author.DeleteAll();
        }
    }
}
