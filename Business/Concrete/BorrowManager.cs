using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class BorrowManager : IBorrowService
    {
        private readonly IBorrowDal _borrowDal;
        private readonly IBookDal _bookDal;
        private readonly IUserDal _userDal;

        public BorrowManager(IBookDal bookDal, IBorrowDal borrowDal, IUserDal userDal)
        {
            _bookDal = bookDal;
            _borrowDal = borrowDal;
            _userDal = userDal;
        }

        public List<Borrow> GetByUserId(int userId)
        {
            return _borrowDal.GetByUserId(userId);
        }

        public void BorrowBook(int userId, int bookId)
        {
            var activeBorrows = _borrowDal.GetByUserId(userId).Count(b => !b.ReturnDate.HasValue);
            // Select COUNT(*) from Borrows where UserId = @userId and ReturnDate Is null


            if (activeBorrows >= 1)
            {
                throw new Exception("Ödünç aldığınız kitabı iade etmeden başka bir kitap alamazsınız");
            }
            var book = _bookDal.GetById(bookId);
            if (book == null || !book.IsAvailable)
            {
                throw new Exception("Bu kitap ödünç alınamaz");
            }
            book.IsAvailable = false;
            _bookDal.Update(book);

            var borrow = new Borrow
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                ReturnDate = null
            };
            _borrowDal.Add(borrow);
        }

        public void ReturnBook(int borrowId)
        {
            var borrow = _borrowDal.GetAll().FirstOrDefault(b => b.Id == borrowId);
            if (borrow == null || borrow.ReturnDate != null)
            {
                throw new Exception("Bu kitap zaten iade edilmiş veya bulunamadı.");
            }
            var minReturnDate = borrow.BorrowDate.AddDays(2);
            if (DateTime.Now < minReturnDate)
            {
                throw new Exception($"Kitap ödünç alındıktan sonraki 2 gün içinde iade edilemez.En erken iade tarihi: {minReturnDate.ToShortDateString()}");
            }

            borrow.ReturnDate = DateTime.Now;
            _borrowDal.Update(borrow);

            var book = _bookDal.GetById(borrow.BookId);
            book.IsAvailable = true;
            _bookDal.Update(book);
        }

        public List<Borrow> GetActiveBorrows()
        {
            return _borrowDal.GetAllWithDetails().Where(b => !b.ReturnDate.HasValue).ToList();
        }
        public List<(Book Book, int BorrowCount)> GetBorrowedBooksRanking()
        {
            return _borrowDal.GetBorrowedBooksRanking();
        }
        public List<UserBorrowStatistics> GetUserBorrowStatistics()
        {
            var users = _userDal.GetAll();
            var statistics = new List<UserBorrowStatistics>();

            foreach (var user in users)
            {
                var borrows = _borrowDal.GetByUserId(user.Id);
                var totalBorrows = borrows.Count;
                var returnedBorrows = borrows.Count(b => b.ReturnDate != null);
                var onTimeReturns = borrows.Count(b => b.ReturnDate != null && b.ReturnDate <= b.DueDate);

                statistics.Add(new UserBorrowStatistics
                {
                    UserId = user.Id,
                    Username = user.Username,
                    TotalBorrows = totalBorrows,
                    ReturnedBorrows = returnedBorrows,// İade edilen kitap sayısı
                    OnTimeReturns = onTimeReturns// zamanında iade
                });
            }

            return statistics.OrderByDescending(s => s.TotalBorrows).ToList();
        }
    }
}
