using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim zorunludur")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler eşleşmiyor")]
        public string ConfirmNewPassword { get; set; }
        public int TotalBorrows { get; set; }
        public int ReturnedBorrows { get; set; }
        public int OnTimeReturns { get; set; }//zamanında teslim sayısı
        public bool IsEmailVerified { get; set; }
        public decimal Debt { get; set; }
    }
}
