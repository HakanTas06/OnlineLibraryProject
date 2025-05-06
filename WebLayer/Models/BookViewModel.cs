using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kitap başlığı zorunludur")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Yazar adı zorunludur")]
        public string Author { get; set; }
        public bool IsAvailable { get; set; }
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçmelisiniz")]
        public int CategoryId { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public double AverageRating { get; set; }

        public string? ImagePath { get; set; } // Resmin yolunu saklar, görüntüleme için.
        [Display(Name = "Kitap Resmi")]
        public IFormFile? ImageFile { get; set; } // Yüklenen resmi almak için.

    }
}
