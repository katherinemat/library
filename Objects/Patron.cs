using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
    public class Patron
    {
        private string _name;
        private int _id;

        public Patron(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }

        public override bool Equals(System.Object otherPatron)
        {
            if(!(otherPatron is Patron))
            {
                return false;
            }
            else
            {
                Patron newPatron = (Patron) otherPatron;
                bool idEquality = this.GetId() == newPatron.GetId();
                bool nameEquality = this.GetName() == newPatron.GetName();
                return (idEquality && nameEquality);
            }
        }

        public int GetId()
        {
            return _id;
        }
        public void SetName(string newName)
        {
            _name = newName;
        }
        public string GetName()
        {
            return _name;
        }

        public static List<Patron> GetAll()
        {
            List<Patron> allPatrons = new List<Patron>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patron", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int patronId = rdr.GetInt32(0);
                string patronName = rdr.GetString(1);
                Patron newPatron = new Patron(patronName, patronId);
                allPatrons.Add(newPatron);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allPatrons;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO patron (name) OUTPUT INSERTED.id VALUES (@Name);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public static Patron Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patron WHERE id = @PatronId;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", id.ToString()));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundPatronId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundPatronId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }
            Patron foundPatron = new Patron(foundName, foundPatronId);

            DB.CloseSqlConnection(rdr, conn);

            return foundPatron;
        }

        public static Patron Search(string name)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patron WHERE name = @PatronName;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronName", name));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundPatronId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundPatronId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }
            Patron foundPatron = new Patron(foundName, foundPatronId);

            DB.CloseSqlConnection(rdr, conn);

            return foundPatron;
        }

        public void UpdateName(string newName)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE patron SET name = @NewName OUTPUT INSERTED.name WHERE id=@PatronId;", conn);

            SqlParameter newNameParameter = new SqlParameter("@NewName", newName);
            cmd.Parameters.Add(newNameParameter);

            SqlParameter patronIdParameter = new SqlParameter("@PatronId", this.GetId());
            cmd.Parameters.Add(patronIdParameter);

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._name = rdr.GetString(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM patron WHERE id = @PatronId;", conn);
            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public void AddCopy(Copy selectedCopy)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO checkout (patron_id, copy_id, current_checkout, due_date) VALUES (@PatronId, @CopyId, @CurrentCheckout, @DueDate); UPDATE copy SET available = @Available WHERE id = @CopyId;", conn);

            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@CopyId", selectedCopy.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@CurrentCheckout", "1"));
            //TODO Later on, lets figure out a way to get the current date to plug into duedate.
            cmd.Parameters.Add(new SqlParameter("@DueDate", "2017-03-02"));
            cmd.Parameters.Add(new SqlParameter("@Available", "0"));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }

            selectedCopy.SetAvailable(0);
        }

        public List<Copy> GetCopy()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT copy.* FROM patron JOIN checkout ON (patron.id = checkout.patron_id) JOIN copy ON (checkout.copy_id = copy.id) WHERE patron.id = @PatronId;", conn);

            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Copy> allCopys = new List<Copy> {};

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

        public List<Copy> GetCurrentCopy()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT copy.* FROM patron JOIN checkout ON (patron.id = checkout.patron_id) JOIN copy ON (checkout.copy_id = copy.id) WHERE patron.id = @PatronId AND current_checkout = @CurrentCheckout;", conn);

            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@CurrentCheckout", "1"));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Copy> allCopys = new List<Copy> {};

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

        public void CheckIn(Copy selectedCopy)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE copy SET available = @Available WHERE id = @CopyId; UPDATE checkout SET current_checkout = @CurrentCheckout WHERE copy_id = @CopyId AND patron_id = @PatronId;", conn);

            cmd.Parameters.Add(new SqlParameter("@Available", "1"));
            cmd.Parameters.Add(new SqlParameter("@CopyId", selectedCopy.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@CurrentCheckout", "0"));
            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }

            selectedCopy.SetAvailable(1);
        }

        public List<string> GetDueDate()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT copy_id, due_date FROM checkout WHERE patron_id = @PatronId AND current_checkout = 1;", conn);

            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<string> allDue = new List<string> {};

            while(rdr.Read())
            {
                int copyId = rdr.GetInt32(0);
                Copy foundCopy = Copy.Find(copyId);
                string foundTitle = foundCopy.GetBookTitle();
                allDue.Add(foundTitle);
                string dueDate = rdr.GetDateTime(1).ToString("yyyy-MM-dd");
                allDue.Add(dueDate);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allDue;
        }

        public List<string> GetOverdue()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT copy_id, due_date FROM checkout WHERE patron_id = @PatronId AND current_checkout = 1 AND due_date < @Today;", conn);

            cmd.Parameters.Add(new SqlParameter("@PatronId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@Today", "2017-03-02"));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<string> allDue = new List<string> {};

            while(rdr.Read())
            {
                int copyId = rdr.GetInt32(0);
                Copy foundCopy = Copy.Find(copyId);
                string foundTitle = foundCopy.GetBookTitle();
                allDue.Add(foundTitle);
                string dueDate = rdr.GetDateTime(1).ToString("yyyy-MM-dd");
                allDue.Add(dueDate);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allDue;
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("patron");
        }
    }
}
