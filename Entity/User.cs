using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public decimal Debt { get; set; }

        public string? PasswordResetToken { get; set; } // Şifre sıfırlama token
        public DateTime? PasswordResetTokenExpiry { get; set; }//geçrlilik sresi

        public string? EmailVerificationToken { get; set; }
        public bool IsEmailVerified { get; set; }//Doğrulama maili
    }
}
