using Nancy;
using System;
using System.Collections.Generic;

namespace Library
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ =>
            {
                return View["patron_login.cshtml"];
            };

            Post["/home"] = _ =>
            {
                //make newPatron object with more information like password and username
                Patron newPatron = new Patron(Request.Form["new-name"]);
                newPatron.Save();
                return View["patron_home.cshtml", newPatron];
            };

            Get["/search/books"] = _ =>
            {
                return View["search_books.cshtml"];
            };

            Post["/search/books"] = _ =>
            {
                Book foundBook = Book.Search(Request.Form["title"]);
                return View["search_books.cshtml", foundBook];
            };

            Get["/checkout/{id}"] = parameters =>
            {
                Book foundBook = Book.Find(parameters.id);
                return View["checkout.cshtml", foundBook];
            };

////////////////////////////////////////////////////////////////


            Get["/admin/login"] = _ =>
            {
                return View["admin_login.cshtml"];
            };

            Post["/admin/home"] = _ =>
            {
                //make newPatron object with more information like password and username
                Patron newPatron = new Patron(Request.Form["admin-new-name"]);
                newPatron.Save();
                return View["admin_home.cshtml", newPatron];
            };

            Get["/admin/books"] = _ =>
            {
                return View["admin_books.cshtml", ModelMaker()];
            };

            Post["/admin/books"] = _ =>
            {
                Author newAuthor = new Author(Request.Form["author"]);
                newAuthor.Save();

                Book newBook = new Book(Request.Form["title"]);
                newBook.Save();
                newBook.AddAuthor(newAuthor.GetId());

                for (int i = 1; i <= Request.Form["copy"]; i++)
                {
                    Copy newCopy = new Copy(newBook.GetId(), i);
                    newCopy.Save();
                }

                return View["admin_books.cshtml", ModelMaker()];
            };

            Get["/admin/books/{id}"] = parameters =>
            {
                Dictionary<string, object> model = new Dictionary<string, object>{};

                Book newBook = Book.Find(parameters.id);
                model.Add("Book", newBook);

                List<Author> newBookAuthor = newBook.GetAuthor();
                model.Add("Authors", newBookAuthor);

                List<Copy> newBookCopies = newBook.GetCopy();
                model.Add("Copies", newBookCopies);

                return View["book.cshtml", model];
            };
// that works -V
            Delete["/admin/book/{id}"] = parameters =>
            {
                Book selectedBook = Book.Find(parameters.id);
                selectedBook.Delete();

                return View["admin_books.cshtml", ModelMaker()];
            };

            Patch["/admin/book/{id}"] = parameters =>
            {
                Book selectedBook = Book.Find(parameters.id);
                selectedBook.UpdateTitle(Request.Form["name"]);

                return View["admin_books.cshtml", ModelMaker()];
            };

            // Post["/admin/home"] = _ =>
            // {
            //     model =
            //     return view["admin_home.cshtml", ModelMaker()];
            // };

        }
        public static Dictionary<string, object> ModelMaker()
        {
            Dictionary<string, object> model = new Dictionary<string, object>{};
            model.Add("Books", Book.GetAll());
            model.Add("Authors", Author.GetAll());
            return model;
        }
    }
}
