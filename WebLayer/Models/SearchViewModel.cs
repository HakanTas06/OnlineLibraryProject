using Entity;

namespace WebLayer.Models
{
    public class SearchViewModel
    {
        public string SearchTitle { get; set; }
        public string SearchAuthor { get; set; }
        public int? SelectedCategoryId { get; set; }
        public List<BookViewModel> Books { get; set; }
        public List<Category> Categories { get; set; }
    }
}
