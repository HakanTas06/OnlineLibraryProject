namespace WebLayer.Models
{
    public class BorrowViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string Username { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public int OverdueDays // Geç teslim edilip edilmediği
        {
            get // dinamik değer hesapladığı için get propu kullanılır.
            {
                if (ReturnDate.HasValue) // Kitap iade edilmişse
                {
                    if (ReturnDate.Value > DueDate)
                    {
                        return (ReturnDate.Value - DueDate).Days;
                    }
                    return 0;
                }
                else
                {
                    if (DueDate < DateTime.Now) // Son teslim tarihi geçmişse
                    {
                        return (DateTime.Now - DueDate).Days;
                    }
                    return 0;
                }
            }
        }
        public decimal Fine => OverdueDays * 5;
        public decimal Debt { get; set; }
    }
}
