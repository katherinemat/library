using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
    public class PatronTest : IDisposable
    {
        public PatronTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_PatronsEmptyAtFirst()
        {
            //Arrange, Act
            int result = Patron.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToPatronObject()
        {
            //Arrange
            Patron testPatron = new Patron("Britton");
            testPatron.Save();

            //Act
            Patron savedPatron = Patron.GetAll()[0];

            int result = savedPatron.GetId();
            int testId = testPatron.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        [Fact]
        public void Test_Find_FindsPatronInDatabase()
        {
            //Arrange
            Patron testPatron1 = new Patron("example patron1");
            testPatron1.Save();

            Patron testPatron2 = new Patron("example patron2");
            testPatron2.Save();

            //Act
            Patron result = Patron.Find(testPatron2.GetId());

            //Assert
            Assert.Equal(testPatron2, result);
        }

        [Fact]
        public void Search_PatronTitle_FoundPatronInDatabase()
        {
            //Arrange
            Patron testPatron1 = new Patron("example patron1");
            testPatron1.Save();

            Patron testPatron2 = new Patron("example patron2");
            testPatron2.Save();

            //Act
            Patron result = Patron.Search(testPatron2.GetName());

            //Assert
            Assert.Equal(testPatron2, result);
        }

        [Fact]
        public void UpdateName_OnePatron_NewName()
        {
            Patron patron1 = new Patron("Barcelona");
            patron1.Save();
            Patron patron2 = new Patron("Honolulu");
            patron2.Save();

            patron1.UpdateName("ex patron");

            string newName = patron1.GetName();

            Assert.Equal(newName, "ex patron");
        }

        [Fact]
        public void Delete_OnePatron_PatronDeleted()
        {
            Patron testPatron1 = new Patron("Britton");
            testPatron1.Save();
            Patron testPatron2 = new Patron("Megan Britney");
            testPatron2.Save();

            testPatron1.Delete();

            List<Patron> allPatrons = Patron.GetAll();
            List<Patron> expected = new List<Patron>{testPatron2};

            Assert.Equal(expected, allPatrons);
        }

        [Fact]
        public void AddCopy_OneCopy_PatronAndNewCopy()
        {
            Patron testPatron = new Patron("Britton");
            testPatron.Save();

            Copy newCopy = new Copy(1, 15);
            newCopy.Save();

            testPatron.AddCopy(newCopy);

            List<Copy> allCopies = testPatron.GetCopy();
            int checkedOut = allCopies[0].GetBookId();

            int expected = newCopy.GetBookId();

            Assert.Equal(expected, checkedOut);
        }

        [Fact]
        public void AddCopy_OneCopy_CopyNotAvailable()
        {
            Patron testPatron = new Patron("Britton");
            testPatron.Save();

            Copy newCopy = new Copy(1, 15);
            newCopy.Save();

            testPatron.AddCopy(newCopy);

            int available = newCopy.GetAvailable();

            Assert.Equal(0, available);
        }

        [Fact]
        public void AddCopy_OneCopy_CopyNotAvailable2()
        {
            Patron testPatron = new Patron("Britton");
            testPatron.Save();
            Patron testPatron2 = new Patron("dkjhfksjdhf");
            testPatron2.Save();

            Book newBook = new Book("super book");

            Copy newCopy = new Copy(newBook.GetId(), 1);
            newCopy.Save();

            testPatron.AddCopy(newCopy);
            testPatron2.AddCopy(newCopy);

            List<Copy> patroncopy = testPatron2.GetCopy();
            List<Copy> expected = new List<Copy>{};

            Assert.Equal(expected, patroncopy);
        }

        [Fact]
        public void CheckIn_OneCopy_PatronAndNewCopy()
        {
            Patron testPatron = new Patron("Britton");
            testPatron.Save();

            Copy newCopy = new Copy(1, 15);
            newCopy.Save();

            testPatron.AddCopy(newCopy);
            testPatron.CheckIn(newCopy);

            List<Copy> allCopies = testPatron.GetCurrentCopy();
            int checkedOut = allCopies.Count;

            Assert.Equal(0, checkedOut);
        }

        [Fact]
        public void CheckIn_OneCopy_CopyAvailable()
        {
            Patron testPatron = new Patron("Britton");
            testPatron.Save();

            Copy newCopy = new Copy(1, 15);
            newCopy.Save();

            testPatron.AddCopy(newCopy);
            testPatron.CheckIn(newCopy);

            int available = newCopy.GetAvailable();

            Assert.Equal(1, available);
        }

        [Fact]
        public void GetDueDate_Patron_oneDatesAndBookTitles()
        {
            Patron newPatron = new Patron("Britton");
            newPatron.Save();

            Book newBook = new Book("the book");
            newBook.Save();

            Copy newCopy = new Copy(newBook.GetId(), 15);
            newCopy.Save();

            newPatron.AddCopy(newCopy);

            List<string> expected = new List<string>{"the book", "2017-03-02"};
            List<string> dueDates = newPatron.GetDueDate();
            Assert.Equal(expected, dueDates);
        }

        [Fact]
        public void GetDueDate_Patron_allDatesAndBookTitles()
        {
            Patron newPatron = new Patron("Britton");
            newPatron.Save();

            Book newBook1 = new Book("the book");
            newBook1.Save();
            Book newBook2 = new Book("the book2");
            newBook2.Save();

            Copy newCopy1 = new Copy(newBook1.GetId(), 15);
            newCopy1.Save();
            Copy newCopy2 = new Copy(newBook2.GetId(), 15);
            newCopy2.Save();

            newPatron.AddCopy(newCopy1);
            newPatron.AddCopy(newCopy2);

            List<string> expected = new List<string>{"the book", "2017-03-02", "the book2", "2017-03-02"};
            List<string> dueDates = newPatron.GetDueDate();
            Assert.Equal(expected, dueDates);
        }

        [Fact]
        public void GetOverdue_Patron_AllDatesAndBookTitles()
        {
            Patron newPatron = new Patron("Britton");
            newPatron.Save();

            Book newBook1 = new Book("the book");
            newBook1.Save();
            Book newBook2 = new Book("the book2");
            newBook2.Save();

            Copy newCopy1 = new Copy(newBook1.GetId(), 15);
            newCopy1.Save();
            Copy newCopy2 = new Copy(newBook2.GetId(), 15);
            newCopy2.Save();

            newPatron.AddCopy(newCopy1);
            newPatron.AddCopy(newCopy2);

            List<string> expected = new List<string>{};
            List<string> dueDates = newPatron.GetOverdue();
            Assert.Equal(expected, dueDates);
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
