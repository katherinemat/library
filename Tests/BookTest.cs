using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
    public class BookTest : IDisposable
    {
        public BookTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_BooksEmptyAtFirst()
        {
            //Arrange, Act
            int result = Book.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToBookObject()
        {
            //Arrange
            Book testBook = new Book("Britton");
            testBook.Save();

            //Act
            Book savedBook = Book.GetAll()[0];

            int result = savedBook.GetId();
            int testId = testBook.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        public void Dispose()
        {
            Book.DeleteAll();
        }
    }
}
