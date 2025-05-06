using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        void Register(User user);
        User Login(string username, string password);
        void Update(User user);
        List<User> GetAll();
        User GetById(int id);
        User GetByEmail(string email);
        User GetByResetToken(string token);
        void VerifyEmail(string token);
    }
}
