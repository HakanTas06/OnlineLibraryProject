using DataAccess.Abstract;
using Entity;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfUserDal : IUserDal
    {
        private readonly LibraryContext _context;

        public EfUserDal(LibraryContext context)
        {
            _context = context;
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User? GetByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            if (_context == null || _context.Users == null)
            {
                throw new InvalidOperationException("Veritabanı başlatılmadı.");
            }

            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }
        public User? GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            if (_context == null || _context.Users == null)
            {
                throw new InvalidOperationException("Veritabanı başlatılmadı.");
            }

            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public User GetByResetToken(string token)
        {
            return _context.Users.FirstOrDefault(u => u.PasswordResetToken == token);
        }
        public User? GetByVerificationToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            return _context.Users.FirstOrDefault(u => u.EmailVerificationToken == token);
        }
    }
}
