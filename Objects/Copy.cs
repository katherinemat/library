using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
    public class Copy
    {
        private int _bookId;
        private int _copyNumber;
        private int _id;
        private int _available;

        public Copy(int bookId, int copyNumber, int id = 0)
        {
            _bookId = bookId;
            _copyNumber = copyNumber;
            _available = 1;
            _id = id;
        }

        public override bool Equals(System.Object otherCopy)
        {
            if(!(otherCopy is Copy))
            {
                return false;
            }
            else
            {
                Copy newCopy = (Copy) otherCopy;
                bool idEquality = this.GetId() == newCopy.GetId();
                bool bookIdEquality = this.GetBookId() == newCopy.GetBookId();
                bool copyNumberEquality = this.GetCopyNumber() == newCopy.GetCopyNumber();
                bool availableEquality = this.GetAvailable() == newCopy.GetAvailable();
                return (idEquality && bookIdEquality && copyNumberEquality && availableEquality);
            }
        }

        public int GetId()
        {
            return _id;
        }
        public void SetId(int newId)
        {
            _id = newId;
        }

        public int GetBookId()
        {
            return _bookId;
        }
        public void SetBookId(int newBookId)
        {
            _bookId = newBookId;
        }

        public int GetCopyNumber()
        {
            return _copyNumber;
        }
        public void SetCopyNumber(int newCopyNumber)
        {
            _copyNumber = newCopyNumber;
        }

        public int GetAvailable()
        {
            return _available;
        }
        public void SetAvailable(int newAvailable)
        {
            _available = newAvailable;
        }

        public static List<Copy> GetAll()
        {
            List<Copy> allCopys = new List<Copy>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copy", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int copyId = rdr.GetInt32(0);
                int copyBookId = rdr.GetInt32(1);
                int copyCopyNumber = rdr.GetInt32(2);
                Copy newCopy = new Copy(copyBookId, copyCopyNumber, copyId);
                allCopys.Add(newCopy);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCopys;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO copy (book_id, copy_number, available) OUTPUT INSERTED.id VALUES (@BookId, @CopyNumber, @Available);", conn);

            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetBookId()));
            cmd.Parameters.Add(new SqlParameter("@CopyNumber", this.GetCopyNumber()));
            cmd.Parameters.Add(new SqlParameter("@Available", "1"));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public static Copy Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copy WHERE id = @CopyId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CopyId", id.ToString()));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundId = 0;
            int foundBookId = 0;
            int foundCopyNumber = 0;

            while(rdr.Read())
            {
                foundId = rdr.GetInt32(0);
                foundBookId = rdr.GetInt32(1);
                foundCopyNumber = rdr.GetInt32(2);
            }
            Copy foundCopy = new Copy(foundBookId, foundCopyNumber, foundId);

            DB.CloseSqlConnection(rdr, conn);

            return foundCopy;
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM copy WHERE id = @CopyId;", conn);
            cmd.Parameters.Add(new SqlParameter("@CopyId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("copy");
        }
    }
}
