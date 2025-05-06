using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifreyi tekrar giriniz")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
