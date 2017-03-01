using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
    public class CopyTest : IDisposable
    {
        public CopyTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Test_CopysEmptyAtFirst()
        {
            //Arrange, Act
            int result = Copy.GetAll().Count;

            //Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Test_Save_AssignsIdToCopyObject()
        {
            //Arrange
            Copy testCopy = new Copy(1, 15);
            testCopy.Save();

            //Act
            Copy savedCopy = Copy.GetAll()[0];

            int result = savedCopy.GetId();
            int testId = testCopy.GetId();

            //Assert
            Assert.Equal(testId, result);
        }

        [Fact]
        public void Test_Find_FindsCopyInDatabase()
        {
            //Arrange
            Copy testCopy1 = new Copy(1, 15);
            testCopy1.Save();
            Copy testCopy2 = new Copy(2, 20);
            testCopy2.Save();

            //Act
            Copy result = Copy.Find(testCopy2.GetId());

            //Assert
            Assert.Equal(testCopy2, result);
        }

        [Fact]
        public void Delete_OneCopy_CopyDeleted()
        {
            Copy testCopy1 = new Copy(1, 15);
            testCopy1.Save();
            Copy testCopy2 = new Copy(2, 20);
            testCopy2.Save();

            testCopy1.Delete();

            List<Copy> allCopys = Copy.GetAll();
            List<Copy> expected = new List<Copy>{testCopy2};

            Assert.Equal(expected, allCopys);
        }

        public void Dispose()
        {
            Copy.DeleteAll();
        }
    }
}
