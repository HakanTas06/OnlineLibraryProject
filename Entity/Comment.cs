using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }// yorum tarihi desc
        public int Rating { get; set; }
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
