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

        public void Dispose()
        {
            Patron.DeleteAll();
        }
    }
}
