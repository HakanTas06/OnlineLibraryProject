using DataAccess.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfCommentDal : ICommentDal
    {
        private readonly LibraryContext _context;
        public EfCommentDal(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public List<Comment> GetCommentsByBookId(int bookId)
        {
            return _context.Comments.Include(c => c.User).Where(c => c.BookId == bookId).ToList();
        }
        public Comment? GetById(int id)
        {
            return _context.Comments
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);
        }
        public void Delete(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }
    }
}
