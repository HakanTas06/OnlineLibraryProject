using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "İsim zorunludur")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim zorunludur")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifreyi tekrar giriniz")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor..")]
        public string ConfirmPassword { get; set; }
    }
}
