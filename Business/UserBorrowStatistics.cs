using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UserBorrowStatistics
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalBorrows { get; set; } // Toplam ödünç alınan kitap sayısı
        public int ReturnedBorrows { get; set; } // İade edilen kitap sayısı
        public int OnTimeReturns { get; set; } // Zamanında iade edilen kitap sayısı,DueDate’den önce veya aynı gün
        public decimal Debt { get; set; }
    }
}
