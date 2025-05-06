using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Borrow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }// Ödünç alma tarihi
        public DateTime? ReturnDate { get; set; }// İade trh.
        public DateTime DueDate { get; set; }// İade edilmesi gereken tarih
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
