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

        [Fact]
        public void Delete_OneBook_BookDeleted()
        {
            Book testBook1 = new Book("Britton");
            testBook1.Save();
            Book testBook2 = new Book("Jungle Book");
            testBook2.Save();

            testBook1.Delete();

            List<Book> allBooks = Book.GetAll();
            List<Book> expected = new List<Book>{testBook2};

            Assert.Equal(expected, allBooks);
        }

        public void Dispose()
        {
            Book.DeleteAll();
        }
    }
}
