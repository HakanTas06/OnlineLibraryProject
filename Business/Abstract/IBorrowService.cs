using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBorrowService
    {
        List<Borrow> GetByUserId(int userId);
        void BorrowBook(int userId, int bookId);
        void ReturnBook(int borrowId);
        List<Borrow> GetActiveBorrows();
        List<(Book Book, int BorrowCount)> GetBorrowedBooksRanking();
        List<UserBorrowStatistics> GetUserBorrowStatistics();
    }
}
