using Business.Abstract;
using Entity;
using Microsoft.AspNetCore.Mvc;
using WebLayer.Models;

namespace WebLayer.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        private readonly IBorrowService _borrowService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookService bookService, IBorrowService borrowService, ICategoryService categoryService, IUserService userService, ICommentService commentService, IWebHostEnvironment webHostEnvironment)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _userService = userService;
            _commentService = commentService;
            _borrowService = borrowService;
            _webHostEnvironment = webHostEnvironment;
        }
        private bool HasFullAccess()
        {
            return User.IsInRole("Admin"); //    HttpContext.User üzerinden mevcut kullanıcının rolünü kontrol eder.
            //User, ClaimsPrincipal üzerinden gelir ve Login metodunda ClaimTypes.Role ile "Admin" olarak ayarlanır.
        }
        [HttpGet]
        public IActionResult Index()
        {
            var books = _bookService.GetAll();
            var model = books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                IsAvailable = b.IsAvailable,
                CategoryName = b.Category?.Name ?? "Bilinmiyor",
                CategoryId = b.CategoryId,
                ImagePath = b.ImagePath
            }).ToList();

            ViewBag.Categories = _categoryService.GetAll();
            var borrowedBooksRanking = _borrowService.GetBorrowedBooksRanking();
            ViewBag.BorrowedBooksRanking = borrowedBooksRanking.Any()
                ? borrowedBooksRanking.Take(3).Select(b => $"{b.Book.Title} - {b.Book.Category.Name} - {b.BorrowCount} kez ödünç alındı").ToList()
                : null;
            return View(model);
        }

        [HttpGet]
        public IActionResult Search()
        {
            var model = new SearchViewModel
            {
                Categories = _categoryService.GetAll(),
                Books = new List<BookViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Search(SearchViewModel model)
        {
            List<Book> books;
            if (!string.IsNullOrEmpty(model.SearchTitle))
            {
                books = _bookService.SearchByTitle(model.SearchTitle);
            }
            else if (!string.IsNullOrEmpty(model.SearchAuthor))
            {
                books = _bookService.SearchByAuthor(model.SearchAuthor);
            }
            else if (model.SelectedCategoryId.HasValue)
            {
                books = _bookService.SearchByCategory(model.SelectedCategoryId.Value);
            }
            else
            {
                books = _bookService.GetAll();
            }

            model.Books = books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                IsAvailable = b.IsAvailable,
                CategoryName = b.Category != null ? b.Category.Name : "Bilinmiyor",
                CategoryId = b.CategoryId,
                ImagePath = b.ImagePath
            }).ToList();

            model.Categories = _categoryService.GetAll();
            model.SearchTitle = null;
            model.SearchAuthor = null;
            model.SelectedCategoryId = null;
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(BookViewModel model)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            ModelState.Remove("CategoryName");
            ModelState.Remove("Comments");
            ModelState.Remove("ImagePath");
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService?.GetAll() ?? new List<Category>();
                return View(model);
            }

            string? imagePath = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");//webhost.. klasör yolunu belirlemek için
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
                imagePath = "/images/" + uniqueFileName; // dosyayı images klasörüne kaydeder.
            }

            var book = new Book
            {
                Title = model.Title ?? "",
                Author = model.Author ?? "",
                IsAvailable = true,
                CategoryId = model.CategoryId,
                ImagePath = imagePath
            };
            _bookService.Add(book, true); // service IsAdmin yetkisi için true
            TempData["Message"] = "Kitap başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            var model = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                IsAvailable = book.IsAvailable,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name ?? "Bilinmiyor",
                ImagePath = book.ImagePath
            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(BookViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Kitap güncelleme modeli boş olamaz.");
                //HTTP 400 hatasını ifade eder, istemciden gelen verilerin yanlış olduğu durumlarda kullanılır.
            }
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            ModelState.Remove("ImageFile");
            ModelState.Remove("CategoryName");
            ModelState.Remove("Comments");
            ModelState.Remove("ImagePath");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService?.GetAll() ?? new List<Category>();
                return View(model);
            }
            var existingBook = _bookService.GetById(model.Id);
            if (existingBook == null)
            {
                return NotFound();
            }

            string? imagePath = existingBook.ImagePath; // resim yolunu korur
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
                imagePath = "/images/" + uniqueFileName;

                if (!string.IsNullOrEmpty(existingBook.ImagePath))
                {
                    //eski yolun başındaki / karakter silinir dosyada var olup olmadığı kontrol edilir varsa silinir.
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingBook.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);// eski resim silme
                    }
                }
            }
            existingBook.Title = model.Title ?? "";
            existingBook.Author = model.Author ?? "";
            existingBook.IsAvailable = model.IsAvailable;
            existingBook.CategoryId = model.CategoryId;
            existingBook.ImagePath = imagePath;

            _bookService.Update(existingBook, true);
            TempData["Message"] = "Kitap başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }

            var comments = _commentService.GetCommentsByBookId(id);
            var model = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                IsAvailable = book.IsAvailable,
                CategoryName = book.Category?.Name ?? "Bilinmiyor",
                CategoryId = book.CategoryId,
                ImagePath = book.ImagePath,
                Comments = comments
                    .OrderByDescending(c => c.CommentDate)
                    .Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        Username = c.User.Username,
                        Content = c.Content,
                        CommentDate = c.CommentDate,
                        Rating = c.Rating
                    }).ToList(),
                AverageRating = comments.Any() ? comments.Average(c => c.Rating) : 0, //ort puan
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddComment(int bookId, string content, int rating)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _commentService.AddComment(userId.Value, bookId, content, rating);
                TempData["Message"] = "Yorum başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Yorum eklenemedi: {ex.Message}";
            }
            return RedirectToAction("Details", new { id = bookId });
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            var book = _bookService.GetById(id);
            if (book == null) return NotFound(); // kod tek satırsa if blok paranteze gerek yok
            var model = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                IsAvailable = book.IsAvailable,
                CategoryName = book.Category.Name
            };
            return View(model);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken] // saldırıları önlemek için
        public IActionResult DeleteConfirmed(int id)
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            try
            {
                _bookService.Delete(id, true);
                TempData["Message"] = "Kitap başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Kitap silinirken hata oluştu: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public IActionResult DeleteComment(int commentId, int bookId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                _commentService.DeleteComment(commentId, userId.Value, User.IsInRole("Admin"));
                TempData["Message"] = "Yorum başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = bookId });
        }
        [HttpGet]
        public IActionResult ActiveBorrows()
        {
            if (!HasFullAccess())
            {
                return RedirectToAction("Index");
            }
            var borrows = _borrowService.GetActiveBorrows()
                .Select(b => new BorrowViewModel
                {
                    Id = b.Id,
                    BookTitle = b.Book.Title,
                    Username = b.User.Username,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate,
                    DueDate = b.DueDate,
                    Debt = _userService.GetById(b.User.Id)?.Debt ?? 0
                }).ToList();
            var borrowedBooksRanking = _borrowService.GetBorrowedBooksRanking(); // top 3 kayıt için
            ViewBag.BorrowedBooksRanking = borrowedBooksRanking.Any()
                ? borrowedBooksRanking.Select(b => $"{b.Book.Title} - {b.BorrowCount} kez ödünç alındı").ToList()
                : null;

            return View(borrows);
        }
    }
}
