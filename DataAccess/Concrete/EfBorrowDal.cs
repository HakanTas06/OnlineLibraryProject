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
    public class EfBorrowDal : IBorrowDal
    {
        private readonly LibraryContext _context;

        public EfBorrowDal(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Borrow borrow)
        {
            _context.Borrows.Add(borrow);
            _context.SaveChanges();
        }

        public List<Borrow> GetAll()
        {
            return _context.Borrows.Include(b => b.Book).ToList();
        }

        public List<Borrow> GetByUserId(int userId)
        {
            return _context.Borrows.Include(b => b.Book)
                   .Where(b => b.UserId == userId)
                   .ToList();
        }

        public void Update(Borrow borrow)
        {
            _context.Borrows.Update(borrow);
            _context.SaveChanges();
        }
        public IQueryable<Borrow> GetAllWithDetails()
        {
            return _context.Borrows.Include(b => b.User).Include(b => b.Book);// IQueryable, sadece gerektiğinde detay için sorgu yapar.
        }
        public List<(Book Book, int BorrowCount)> GetBorrowedBooksRanking()
        {
            var borrowCounts = _context.Borrows
                .GroupBy(b => b.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .ToList();

            if (!borrowCounts.Any())
            {
                return new List<(Book, int)>(); //ödünç alınan kitap yoksa boş liste
            }

            var bookIds = borrowCounts.Select(bc => bc.BookId).ToList();
            var books = _context.Books
                .Include(b => b.Category)
                .Where(b => bookIds.Contains(b.Id))
                .ToDictionary(b => b.Id, b => b);
            // ValueTuple, 2 veya daha fazla değeri gruplar. BookId'ye göre kitapların kaç kez ödünç alındığı bilgisi..
            return borrowCounts.Select(bc => new ValueTuple<Book, int>(books[bc.BookId], bc.BorrowCount)).ToList();
        }
    }
}
