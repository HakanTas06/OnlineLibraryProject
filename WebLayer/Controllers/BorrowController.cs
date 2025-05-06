using Business.Abstract;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Mvc;
using WebLayer.Models;

namespace WebLayer.Controllers
{
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;

        public BorrowController(IBorrowService borrowService, IBookService bookService, IUserService userService)
        {
            _borrowService = borrowService;
            _bookService = bookService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var borrows = _borrowService.GetByUserId(userId.Value);
            var model = borrows.Select(b => new BorrowViewModel
            {
                Id = b.Id,
                BookTitle = b.Book.Title,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate
            }).ToList();
            var user = _userService.GetById(userId.Value);
            var overdueBorrows = model.Where(b => b.OverdueDays > 0).ToList(); // gecikmiş kayıtlar için borç hesaplama
            if (overdueBorrows.Any() && user.Debt > 0)
            {
                TempData["OverdueWarning"] = $"Dikkat! {overdueBorrows.Count} kitabınızın iade süresi geçti. Toplam ceza: {overdueBorrows.Sum(b => b.Fine)} TL";
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult BorrowBook(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var book = _bookService.GetById(bookId);
            if (book == null || !book.IsAvailable)
            {
                return RedirectToAction("Index", "Book");
            }

            var model = new BorrowViewModel
            {
                Id = bookId,
                BookTitle = book.Title
            };
            TempData["BorrowInfo"] = "Hatırlatma: İade gecikme süresi günlük 5 TL’dir.";
            return View(model);
        }

        [HttpPost]
        public IActionResult BorrowBook(BorrowViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _borrowService.BorrowBook(userId.Value, model.Id);
                TempData["Message"] = "Kitap başarıyla ödünç alındı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ödünç alma başarısız: {ex.Message}";
            }
            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult ReturnBook(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var borrow = _borrowService.GetByUserId(userId.Value).FirstOrDefault(b => b.Id == id);
            if (borrow == null || borrow.ReturnDate.HasValue) // iade edilmişse
            {
                return RedirectToAction("Index");
            }

            var model = new BorrowViewModel
            {
                Id = borrow.Id,
                BookTitle = borrow.Book.Title,
                BorrowDate = borrow.BorrowDate,
                DueDate = borrow.DueDate,
                ReturnDate = borrow.ReturnDate
            };
            return View(model);
        }

        [HttpPost, ActionName("ReturnBook")]
        public IActionResult ReturnBookConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                TempData["Error"] = null;

                var user = _userService.GetById(userId.Value);
                if (!User.IsInRole("Admin") && user.Debt > 0)
                {
                    TempData["Error"] = $"İade işlemi yapılamadı. Ödenmemiş {user.Debt} TL borcunuz var. Lütfen borcunuzu ödeyin.";
                    return RedirectToAction("Index");
                }

                var borrow = _borrowService.GetByUserId(userId.Value).FirstOrDefault(b => b.Id == id);
                if (borrow == null)
                {
                    throw new Exception("Ödünç kaydı bulunamadı.");
                }

                if (borrow.ReturnDate.HasValue)
                {
                    throw new Exception("Bu kitap zaten iade edilmiş.");
                }

                var today = DateTime.Now;
                decimal fine = 0m;
                if (borrow.DueDate < today)
                {
                    var overdueDays = (today - borrow.DueDate).Days;
                    if (overdueDays > 0)
                    {
                        fine = overdueDays * 5m;
                        if (!User.IsInRole("Admin"))
                        {
                            user.Debt += fine;
                            _userService.Update(user);
                        }
                    }
                }
                _borrowService.ReturnBook(id);
                user = _userService.GetById(userId.Value);
                if (fine > 0 && user.Debt != fine && !User.IsInRole("Admin"))//borç ve ceza uyuşmuyorsa, admine yetki borcu silmek için
                {
                    user.Debt = fine;
                    _userService.Update(user);
                }
                TempData["Message"] = "Kitap başarıyla iade edildi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
