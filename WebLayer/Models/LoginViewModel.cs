using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Parola zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
