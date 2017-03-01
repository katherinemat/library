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

        public static void DeleteAll()
        {
            DB.TableDeleteAll("author");
        }
    }
}
