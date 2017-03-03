using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
    public class Book
    {
        private string _title;
        private int _id;

        public Book(string title, int id = 0)
        {
            _title = title;
            _id = id;
        }

        public override bool Equals(System.Object otherBook)
        {
            if(!(otherBook is Book))
            {
                return false;
            }
            else
            {
                Book newBook = (Book) otherBook;
                bool idEquality = this.GetId() == newBook.GetId();
                bool titleEquality = this.GetTitle() == newBook.GetTitle();
                return (idEquality && titleEquality);
            }
        }

        public int GetId()
        {
            return _id;
        }
        public void SetTitle(string newTitle)
        {
            _title = newTitle;
        }
        public string GetTitle()
        {
            return _title;
        }

        public static List<Book> GetAll()
        {
            List<Book> allBooks = new List<Book>{};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM book", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                int bookId = rdr.GetInt32(0);
                string bookTitle = rdr.GetString(1);
                Book newBook = new Book(bookTitle, bookId);
                allBooks.Add(newBook);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allBooks;
        }

        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO book (title) OUTPUT INSERTED.id VALUES (@Title);", conn);

            cmd.Parameters.Add(new SqlParameter("@Title", this.GetTitle()));

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public static Book Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM book WHERE id = @BookId;", conn);
            cmd.Parameters.Add(new SqlParameter("@BookId", id.ToString()));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundBookId = 0;
            string foundTitle = null;

            while(rdr.Read())
            {
                foundBookId = rdr.GetInt32(0);
                foundTitle = rdr.GetString(1);
            }
            Book foundBook = new Book(foundTitle, foundBookId);

            DB.CloseSqlConnection(rdr, conn);

            return foundBook;
        }

        public static Book Search(string title)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM book WHERE title = @BookTitle;", conn);
            cmd.Parameters.Add(new SqlParameter("@BookTitle", title));
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundBookId = 0;
            string foundTitle = null;

            while(rdr.Read())
            {
                foundBookId = rdr.GetInt32(0);
                foundTitle = rdr.GetString(1);
            }
            Book foundBook = new Book(foundTitle, foundBookId);

            DB.CloseSqlConnection(rdr, conn);

            return foundBook;
        }

        public void UpdateTitle(string newTitle)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE book SET title = @NewTitle OUTPUT INSERTED.title WHERE id=@BookId;", conn);

            SqlParameter newTitleParameter = new SqlParameter("@NewTitle", newTitle);
            cmd.Parameters.Add(newTitleParameter);

            SqlParameter bookIdParameter = new SqlParameter("@BookId", this.GetId());
            cmd.Parameters.Add(bookIdParameter);

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._title = rdr.GetString(0);
            }

            DB.CloseSqlConnection(rdr, conn);
        }

        public void Delete()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM book WHERE id = @BookId;", conn);
            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId()));

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public void AddAuthor(int authorId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (book_id, author_id) VALUES (@BookId, @AuthorId);", conn);

            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@AuthorId", authorId.ToString()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public List<Author> GetAuthor()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT author.* FROM book JOIN books_authors ON (book.id = books_authors.book_id) JOIN author ON (books_authors.author_id = author.id) WHERE book.id = @BookId;", conn);

            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Author> allAuthors = new List<Author> {};

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

        public void UpdateAuthor(Author OldAuthor, Author newAuthor)
        {
            int OldAuthorId = OldAuthor.GetId();

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE books_authors SET author_id = @NewAuthorId WHERE author_id = @OldAuthorId AND book_id = @BookId;", conn);

            SqlParameter bookParameter = new SqlParameter("@BookId", this.GetId());
            SqlParameter oldAuthorParameter = new SqlParameter("@OldAuthorId", OldAuthorId);
            SqlParameter newAuthorParameter = new SqlParameter("@NewAuthorId", newAuthor.GetId());
            cmd.Parameters.Add(bookParameter);
            cmd.Parameters.Add(oldAuthorParameter);
            cmd.Parameters.Add(newAuthorParameter);

            SqlDataReader rdr = null;
            cmd.ExecuteNonQuery();

            DB.CloseSqlConnection(rdr, conn);
        }

        public List<Copy> GetCopy()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copy WHERE book_id = @BookId;", conn);

            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId().ToString()));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Copy> allCopies = new List<Copy> {};

            while(rdr.Read())
            {
                int copyId = rdr.GetInt32(0);
                int bookId = rdr.GetInt32(1);
                int copyNumber = rdr.GetInt32(2);
                int available = rdr.GetByte(3);
                Copy newCopy = new Copy(bookId, copyNumber, copyId);
                allCopies.Add(newCopy);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCopies;
        }

        public List<Copy> GetAvailableCopy()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copy WHERE book_id = @BookId AND available = @Available;", conn);

            cmd.Parameters.Add(new SqlParameter("@BookId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@Available", "1"));

            SqlDataReader rdr = cmd.ExecuteReader();

            List<Copy> allCopies = new List<Copy> {};

            while(rdr.Read())
            {
                int copyId = rdr.GetInt32(0);
                int bookId = rdr.GetInt32(1);
                int copyNumber = rdr.GetInt32(2);
                int available = rdr.GetByte(3);
                Copy newCopy = new Copy(bookId, copyNumber, copyId);
                allCopies.Add(newCopy);
            }

            DB.CloseSqlConnection(rdr, conn);

            return allCopies;
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("book");
        }
    }
}
