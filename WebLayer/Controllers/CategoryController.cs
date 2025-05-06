using Business.Abstract;
using Entity;
using Microsoft.AspNetCore.Mvc;

namespace WebLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }
            var categories = _categoryService.GetAll();
            return View(categories);
        }

        private bool HasFullAccess()
        {
            return User.IsInRole("Admin");
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }
            ModelState.Remove("Books");
            if (ModelState.IsValid)
            {
                _categoryService.Add(category);
                TempData["Message"] = "Kategori başarıyla eklendi!";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }
            ModelState.Remove("Books");
            if (ModelState.IsValid)
            {
                _categoryService.Update(category);
                TempData["Message"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }

            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(int id, IFormCollection collection)//get ile ayırmak için collection..
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index", "Book");
            }

            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            _categoryService.Delete(id);
            TempData["Message"] = "Kategori başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}
