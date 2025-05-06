using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IUserDal
    {
        User? GetByUsername(string username);
        void Add(User user);
        User? GetById(int id);
        void Update(User user);
        List<User> GetAll();
        User? GetByEmail(string email);
        User GetByResetToken(string token);
        User? GetByVerificationToken(string token); //doğrulama tokeni
    }
}
