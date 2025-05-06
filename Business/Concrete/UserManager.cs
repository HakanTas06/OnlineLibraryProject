using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public void Register(User user)
        {
            if (user.Password.Length < 6)
            {
                throw new Exception("Şifre en az 6 karakter olmalıdır.");
            }
            var existingUser = _userDal.GetByEmail(user.Email);
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.IsEmailVerified = false;
            _userDal.Add(user);
        }

        public User Login(string username, string password)
        {
            var user = _userDal.GetByUsername(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))//şifre eşleşmezse false,! true
            {
                return null;
            }
            return user;
        }

        public void Update(User user)
        {
            var existingUser = _userDal.GetById(user.Id);
            if (existingUser == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (user.Password != existingUser.Password || !existingUser.Password.StartsWith("$2a$"))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            _userDal.Update(user);
        }
        public List<User> GetAll()
        {
            return _userDal.GetAll();
        }
        public User GetById(int id)
        {
            return _userDal.GetById(id);
        }

        public User GetByEmail(string email)
        {
            return _userDal.GetByEmail(email);
        }
        public User GetByResetToken(string token)
        {
            return _userDal.GetByResetToken(token);
        }
        public void VerifyEmail(string token)
        {
            var user = _userDal.GetByVerificationToken(token);
            if (user == null)
            {
                throw new Exception("Geçersiz doğrulama kodu.");
            }
            if (user.IsEmailVerified)
            {
                throw new Exception("Bu e-posta zaten doğrulanmış.");
            }
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            _userDal.Update(user);
        }
    }
}
