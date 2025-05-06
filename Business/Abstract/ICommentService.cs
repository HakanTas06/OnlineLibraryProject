using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICommentService
    {
        void AddComment(int userId, int bookId, string content, int rating);
        List<Comment> GetCommentsByBookId(int bookId);
        void DeleteComment(int commentId, int userId, bool isAdmin);
    }
}
