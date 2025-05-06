using System.ComponentModel.DataAnnotations;

namespace WebLayer.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [MaxLength(200, ErrorMessage = "Yorum en fazla 200 karakter olabilir")]
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int Rating { get; set; }
    }
}
