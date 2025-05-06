using DataAccess.Abstract;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfCategoryDal : ICategoryDal
    {
        private readonly LibraryContext _context;

        public EfCategoryDal(LibraryContext context)
        {
            _context = context;
        }
        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void Delete(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        public Category GetById(int id)
        {
            return _context.Categories.Find(id);
        }
    }
}
