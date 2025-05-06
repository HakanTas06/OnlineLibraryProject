using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly ICommentDal _commentDal;

        public CommentManager(ICommentDal commentDal)
        {
            _commentDal = commentDal;
        }

        public void AddComment(int userId, int bookId, string content, int rating)
        {
            if (content.Length > 200)
            {
                throw new Exception("Max 200 karakter!");
            }
            if (rating < 1 || rating > 5)
            {
                throw new Exception("Puan 1-5 arasında olmalıdır!");
            }
            var comment = new Comment
            {
                UserId = userId,
                BookId = bookId,
                Content = content,
                CommentDate = DateTime.Now,
                Rating = rating
            };
            _commentDal.Add(comment);
        }

        public List<Comment> GetCommentsByBookId(int bookId)
        {
            return _commentDal.GetCommentsByBookId(bookId);
        }
        public void DeleteComment(int commentId, int userId, bool isAdmin)
        {
            var comment = _commentDal.GetById(commentId);
            if (comment == null)
            {
                throw new Exception("Yorum bulunamadı!");
            }
            if (!isAdmin && comment.UserId != userId)
            {
                throw new Exception("Sadece kendi yorumunuzu silebilirsiniz!");
            }

            _commentDal.Delete(commentId);
        }
    }
}
