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

        [Fact]
        public void Test_Find_FindsAuthorInDatabase()
        {
            //Arrange
            Author testAuthor1 = new Author("example author1");
            testAuthor1.Save();

            Author testAuthor2 = new Author("example author2");
            testAuthor2.Save();

            //Act
            Author result = Author.Find(testAuthor2.GetId());

            //Assert
            Assert.Equal(testAuthor2, result);
        }

        [Fact]
        public void Search_AuthorTitle_FoundAuthorInDatabase()
        {
            //Arrange
            Author testAuthor1 = new Author("example author1");
            testAuthor1.Save();

            Author testAuthor2 = new Author("example author2");
            testAuthor2.Save();

            //Act
            Author result = Author.Search(testAuthor2.GetName());

            //Assert
            Assert.Equal(testAuthor2, result);
        }

        [Fact]
        public void UpdateName_OneAuthor_NewName()
        {
            Author author1 = new Author("Barcelona");
            author1.Save();
            Author author2 = new Author("Honolulu");
            author2.Save();

            author1.UpdateName("ex author");

            string newName = author1.GetName();

            Assert.Equal(newName, "ex author");
        }

        [Fact]
        public void Delete_OneAuthor_AuthorDeleted()
        {
            Author testAuthor1 = new Author("Britton");
            testAuthor1.Save();
            Author testAuthor2 = new Author("Megan Britney");
            testAuthor2.Save();

            testAuthor1.Delete();

            List<Author> allAuthors = Author.GetAll();
            List<Author> expected = new List<Author>{testAuthor2};

            Assert.Equal(expected, allAuthors);
        }

        public void Dispose()
        {
            Author.DeleteAll();
        }
    }
}
