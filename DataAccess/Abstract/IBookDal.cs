using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IBookDal
    {
        List<Book> GetAll();
        Book? GetById(int id);
        void Add(Book book);
        void Update(Book book);
        void Delete(int id);
        List<Book> SearchByTitle(string title);
        List<Book> SearchByCategory(int categoryId);
        List<Book> SearchByAuthor(string author);
    }
}
