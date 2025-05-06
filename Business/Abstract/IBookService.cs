using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBookService
    {
        List<Book> GetAll();
        Book? GetById(int id);
        void Add(Book book, bool isAdmin);
        void Update(Book book, bool isAdmin);
        void Delete(int id, bool isAdmin);
        List<Book> SearchByTitle(string title);
        List<Book> SearchByCategory(int categoryId);
        List<Book> SearchByAuthor(string author);
    }
}
