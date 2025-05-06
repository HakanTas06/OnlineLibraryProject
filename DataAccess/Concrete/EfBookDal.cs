using DataAccess.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfBookDal : IBookDal
    {
        private readonly LibraryContext _context;

        public EfBookDal(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                throw new Exception($"ID {id} ile bir kitap bulunamadı.");
            }

            _context.Books.Remove(book);
            int rowsAffected = _context.SaveChanges();
            if (rowsAffected == 0)
            {
                throw new Exception("Kitap silinirken bir hata oluştu: Hiçbir satır etkilenmedi.");
            }
        }

        public List<Book> GetAll()
        {
            /* Select b.*, c.*  from Books b Inner Join Categories c
               on b.CategoryId = c.Id */
            return _context.Books.Include(b => b.Category).ToList();
        }

        public Book? GetById(int id)
        {
            return _context.Books.Include(b => b.Category)
                .FirstOrDefault(b => b.Id == id);
        }

        public List<Book> SearchByCategory(int categoryId)
        {
            return _context.Books.Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .ToList();
        }

        public List<Book> SearchByTitle(string title)
        {
            /* Select b.*, c.* from Books b Inner Join Categories c
               on b.CategoryId = c.Id
               where LOWER(b.Title) like  '%' + LOWER(@title) + '%'
             */
            return _context.Books.Include(b => b.Category)
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .ToList();
        }
        public List<Book> SearchByAuthor(string author)
        {
            return _context.Books.Include(b => b.Category)
                .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                .ToList();
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }
    }
}
