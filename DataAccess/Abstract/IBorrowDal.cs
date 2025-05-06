using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IBorrowDal
    {
        List<Borrow> GetAll();
        List<Borrow> GetByUserId(int userId);
        void Add(Borrow borrow);
        void Update(Borrow borrow);
        List<(Book Book, int BorrowCount)> GetBorrowedBooksRanking(); // istatistik 
        IQueryable<Borrow> GetAllWithDetails();
    }
}
