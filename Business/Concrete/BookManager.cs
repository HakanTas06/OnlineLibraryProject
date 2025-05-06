using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class BookManager : IBookService
    {
        private readonly IBookDal _bookDal;

        public BookManager(IBookDal bookDal)
        {
            _bookDal = bookDal;
        }
        public void Add(Book book, bool isAdmin)
        {
            if (!isAdmin)
            { 
                throw new Exception("Sadece admin kitap ekleyebilir.");
            }
            _bookDal.Add(book);
        }

        public void Delete(int id, bool isAdmin)
        {
            if (!isAdmin)
            {
                throw new Exception("Bu işlemi yalnızca admin yapabilir.");
            }

            try
            {
                _bookDal.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Kitap silinirken hata oluştu: {ex.Message}");
            }
        }

        public List<Book> GetAll()
        {
            return _bookDal.GetAll();
        }

        public Book? GetById(int id)
        {
            return _bookDal.GetById(id);
        }

        public List<Book> SearchByCategory(int categoryId)
        {
            return _bookDal.SearchByCategory(categoryId);
        }

        public List<Book> SearchByTitle(string title)
        {
            return _bookDal.SearchByTitle(title);
        }
        public List<Book> SearchByAuthor(string author)
        {
            return _bookDal.SearchByAuthor(author);
        }

        public void Update(Book book, bool isAdmin)
        {
            if (!isAdmin)
            {
                throw new Exception("Sadece admin kitap güncelleyebilir");
            }
            _bookDal.Update(book);
        }
    }
}
