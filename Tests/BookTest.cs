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
        public void Test_Find_FindsBookInDatabase()
        {
            //Arrange
            Book testBook1 = new Book("example book1");
            testBook1.Save();

            Book testBook2 = new Book("example book2");
            testBook2.Save();

            //Act
            Book result = Book.Find(testBook2.GetId());

            //Assert
            Assert.Equal(testBook2, result);
        }

        [Fact]
        public void Search_BookTitle_FoundBookInDatabase()
        {
            //Arrange
            Book testBook1 = new Book("example book1");
            testBook1.Save();

            Book testBook2 = new Book("example book2");
            testBook2.Save();

            //Act
            Book result = Book.Search(testBook2.GetTitle());

            //Assert
            Assert.Equal(testBook2, result);
        }

        [Fact]
        public void UpdateTitle_OneBook_NewTitle()
        {
            Book book1 = new Book("Barcelona");
            book1.Save();
            Book book2 = new Book("Honolulu");
            book2.Save();

            book1.UpdateTitle("ex book");

            string newTitle = book1.GetTitle();

            Assert.Equal(newTitle, "ex book");
        }

        [Fact]
        public void Delete_OneBook_BookDeleted()
        {
            Book testBook1 = new Book("Alice in Wonderland");
            testBook1.Save();
            Book testBook2 = new Book("Jungle Book");
            testBook2.Save();

            testBook1.Delete();

            List<Book> allBooks = Book.GetAll();
            List<Book> expected = new List<Book>{testBook2};

            Assert.Equal(expected, allBooks);
        }

        [Fact]
        public void AddAuthor_OneBook_BookAndAuthor()
        {
            Book testBook1 = new Book("Alice in Wonderland");
            testBook1.Save();

            Author testAuthor1 = new Author("example author1");
            testAuthor1.Save();

            Author testAuthor2 = new Author("example author2");
            testAuthor2.Save();

            testBook1.AddAuthor(testAuthor1.GetId());
            testBook1.AddAuthor(testAuthor2.GetId());

            List<Author> allAuthors = testBook1.GetAuthor();
            List<Author> expected = new List<Author>{testAuthor1, testAuthor2};

            Assert.Equal(expected, allAuthors);
        }

        [Fact]
        public void UpdateAuthor_OneBook_BookAndNewAuthor()
        {
            Book testBook1 = new Book("Alice in Wonderland");
            testBook1.Save();

            Author testAuthor1 = new Author("example author1");
            testAuthor1.Save();

            Author testAuthor2 = new Author("example author2");
            testAuthor2.Save();

            testBook1.AddAuthor(testAuthor1.GetId());
            testBook1.UpdateAuthor(testAuthor1, testAuthor2);

            List<Author> allAuthors = testBook1.GetAuthor();
            List<Author> expected = new List<Author>{testAuthor2};

            Assert.Equal(expected, allAuthors);
        }

        [Fact]
        public void GetAvailableCopy_Book_BookAvailableCopies()
        {
            Book testBook1 = new Book("Alice in Wonderland");
            testBook1.Save();

            Copy newCopy1 = new Copy(testBook1.GetId(), 15);
            newCopy1.Save();
            Copy newCopy2 = new Copy(testBook1.GetId(), 12);
            newCopy2.Save();

            Patron testPatron = new Patron("Britton");
            testPatron.Save();
            testPatron.AddCopy(newCopy1);

            List<Copy> availableCopies = testBook1.GetAvailableCopy();
            // int result = availableCopies[0].GetBookId();
            List<Copy> expectedList = new List<Copy>{newCopy2};
            // int expected = expectedList[0].GetBookId();

            Assert.Equal(expectedList, availableCopies);
        }

        public void Dispose()
        {
            Copy.DeleteAll();
            Patron.DeleteAll();
            Book.DeleteAll();
            Author.DeleteAll();
        }
    }
}
