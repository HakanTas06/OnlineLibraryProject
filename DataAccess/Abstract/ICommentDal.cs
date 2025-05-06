using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICommentDal
    {
        void Add(Comment comment);
        List<Comment> GetCommentsByBookId(int bookId);
        Comment? GetById(int id);
        void Delete(int id);
    }
}
