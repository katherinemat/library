using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
    public class Author
    {
        private string _name;
        private int _id;

        public Author(string name, int id = 0)
        {
            _name = name;
            _id = id;
        }

        public override bool Equals(System.Object otherAuthor)
        {
            if(!(otherAuthor is Author))
            {
                return false;
            }
            else
            {
                Author newAuthor = (Author) otherAuthor;
                bool idEquality = this.GetId() == newAuthor.GetId();
                bool nameEquality = this.GetName() == newAuthor.GetName();
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

        public static List<Author> GetAll()
        {
            List<Author> allAuthors = new List<Author>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM author", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int authorId = rdr.GetInt32(0);
                string authorName = rdr.GetString(1);
                Author newAuthor = new Author(authorName, authorId);
                allAuthors.Add(newAuthor);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allAuthors;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO author (name) OUTPUT INSERTED.id VALUES (@Name);", conn);

            cmd.Parameters.Add(new SqlParameter("@Name", this.GetName()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public static Author Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM author WHERE id = @AuthorId;", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorId", id.ToString()));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundAuthorId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundAuthorId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }
            Author foundAuthor = new Author(foundName, foundAuthorId);

            DB.CloseSqlConnection(rdr, conn);

            return foundAuthor;
        }

        public static Author Search(string name)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM author WHERE name = @AuthorName;", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorName", name));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundAuthorId = 0;
            string foundName = null;

            while(rdr.Read())
            {
                foundAuthorId = rdr.GetInt32(0);
                foundName = rdr.GetString(1);
            }
            Author foundAuthor = new Author(foundName, foundAuthorId);

            DB.CloseSqlConnection(rdr, conn);

            return foundAuthor;
        }

        public void UpdateName(string newName)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE author SET name = @NewName OUTPUT INSERTED.name WHERE id=@AuthorId;", conn);

            SqlParameter newNameParameter = new SqlParameter("@NewName", newName);
            cmd.Parameters.Add(newNameParameter);

            SqlParameter authorIdParameter = new SqlParameter("@AuthorId", this.GetId());
            cmd.Parameters.Add(authorIdParameter);

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

            SqlCommand cmd = new SqlCommand("DELETE FROM author WHERE id = @AuthorId;", conn);
            cmd.Parameters.Add(new SqlParameter("@AuthorId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("author");
        }
    }
}
