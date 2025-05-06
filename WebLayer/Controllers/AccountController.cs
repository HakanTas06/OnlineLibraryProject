using Business;
using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WebLayer.EmailServices;
using WebLayer.Models;

namespace WebLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBorrowService _borrowService;
        private readonly IEmailService _emailService;
        public AccountController(IUserService userService, IBorrowService borrowService, IEmailService emailService)
        {
            _userService = userService;
            _borrowService = borrowService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Kullanıcı adı veya parola boş olamaz.";
                return View(model ?? new LoginViewModel()); // ?? = null değilse modele eşit null ise yeni model
            }

            var user = _userService.Login(model.Username, model.Password);
            if (user != null)
            {
                if (!user.IsEmailVerified)
                {
                    ViewBag.Error = "Lütfen önce e-posta adresinizi doğrulayın.";
                    return View(model);
                }
                if (HttpContext.Session == null)
                {
                    ViewBag.Error = "Oturum başlatılamadı, lütfen tekrar deneyin.";
                    return View(model);
                }

                HttpContext.Session.SetString("Username", user.Username ?? "");
                HttpContext.Session.SetInt32("UserId", user.Id);
                TempData["WelcomeMessage"] = $"Hoşgeldin {(user.IsAdmin ? "Admin" : user.Username)}!";

                /* Claim: Kimlik doğrulama ve yetkilendirme sisteminde kullanılan bir veri parçasıdır.
                   Bir kullanıcının belirli bir özelliğini veya rolünü temsil eder. Key-value ilişkisi var. */

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                    new Claim("FullAccess", (user.Username == "admin") ? "True" : "False")// admine tam yetki
                };
                /*ClaimsIdentity, bir kullanıcının kimlik bilgilerini bir paket içinde toplar.
                  CookieAuthenticationDefaults.AuthenticationScheme, cookie tabanlı kimlik doğrulama için bir şema tanımlar.
                  ClaimsPrincipal: Uygulamanın mevcut kullanıcı kimliğini temsil eder.Tüm claim leri içeren bir ana nesnedir ve HttpContext.User ile erişilir.

                  CookieAuthentication: Cookie tabanlı kimlik doğrulama sistemidir.
                  Kullanıcının oturum açma bilgilerini bir tarayıcı çerezinde (cookie) saklar ve her istekte bu çerezi kontrol ederek kimlik doğrular.
                  Kullanıcının giriş durumunu hatırlar. ClaimsPrincipal’ı cookie’de saklar, böylece User.Identity üzerinden erişilebilir.

                 */


                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                //Kullanıcının kimlik bilgilerini bir cookie’ye kaydeder ve oturum açar.
                return RedirectToAction("Index", "Book");
            }

            ViewBag.Error = "Kullanıcı adı veya parola yanlış.";
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var users = _userService.GetAll();
            var model = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Debt = u.Debt,
                IsEmailVerified = u.IsEmailVerified
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userService.GetByEmail(model.Email);
            if (user == null)
            {
                ViewBag.Error = "Bu e-posta adresiyle bir kullanıcı bulunamadı.";
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.Now.AddHours(1);
            _userService.Update(user);

            var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
            var subject = "Şifre Sıfırlama Talebi";
            var body = $@"
                <h2>Merhaba {user.Name},</h2>
                <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
                <p><a href='{resetLink}'>Şifreyi Sıfırla</a></p>
                <p>Bu link 1 saat geçerlidir.</p>
                <p>Saygılar,<br>Online Kütüphane Ekibi</p> ";
            await _emailService.SendEmailAsync(model.Email, subject, body);

            TempData["Message"] = "Şifre sıfırlama linki e-posta adresinize gönderildi.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Geçersiz şifre sıfırlama linki.";
                return RedirectToAction("Login");
            }

            var user = _userService.GetByResetToken(token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.Now)
            {
                TempData["Error"] = "Bu şifre sıfırlama linki geçersiz veya süresi dolmuş.";
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userService.GetByResetToken(model.Token);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.Now)
            {
                TempData["Error"] = "Bu şifre sıfırlama linki geçersiz veya süresi dolmuş.";
                return RedirectToAction("Login");
            }

            user.Password = model.Password;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            _userService.Update(user);

            TempData["Message"] = "Şifreniz başarıyla sıfırlandı. Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    IsAdmin = false,
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email
                };
                _userService.Register(user);

                // E-posta doğrulama linki gönder
                var verificationLink = Url.Action("VerifyEmail", "Account", new { token = user.EmailVerificationToken }, Request.Scheme);
                var subject = "E-posta Doğrulama";
                var body = $@"
                <h2>Merhaba {user.Name},</h2>
                <p>Kayıt olduğunuz için teşekkürler! E-posta adresinizi doğrulamak için aşağıdaki linke tıklayın:</p>
                <p><a href='{verificationLink}'>E-postamı Doğrula</a></p>
                <p>Saygılar,<br>Online Kütüphane Ekibi</p>";
                await _emailService.SendEmailAsync(user.Email, subject, body);

                TempData["Message"] = "Kayıt başarılı! Lütfen e-posta adresinize gönderilen doğrulama linkine tıklayın.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Geçersiz doğrulama linki.";
                return RedirectToAction("Login");
            }

            try
            {
                _userService.VerifyEmail(token);
                TempData["Message"] = "E-posta adresiniz başarıyla doğrulandı! Şimdi giriş yapabilirsiniz.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = _userService.GetById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                TotalBorrows = _borrowService.GetByUserId(userId.Value).Count,
                ReturnedBorrows = _borrowService.GetByUserId(userId.Value).Count(b => b.ReturnDate != null),
                OnTimeReturns = _borrowService.GetByUserId(userId.Value).Count(b => b.ReturnDate != null && b.ReturnDate <= b.DueDate),
                Debt = user.Debt
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UserStatistics()
        {
            var statistics = _borrowService.GetUserBorrowStatistics()
        .Select(stat => new UserBorrowStatistics
        {
            UserId = stat.UserId,
            Username = stat.Username,
            TotalBorrows = stat.TotalBorrows,
            ReturnedBorrows = stat.ReturnedBorrows,
            OnTimeReturns = stat.OnTimeReturns,
            Debt = _userService.GetById(stat.UserId)?.Debt ?? 0
        }).ToList();
            return View(statistics);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Settings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = _userService.GetById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                CurrentPassword = "",
                NewPassword = "",
                ConfirmNewPassword = ""
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Settings(UserViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userService.GetById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.Username != model.Username)
            {
                var existingUser = _userService.GetByEmail(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }
            }

            if (!string.IsNullOrEmpty(model.CurrentPassword) || !string.IsNullOrEmpty(model.NewPassword) || !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    ModelState.AddModelError("", "Şifre değiştirmek için mevcut şifre, yeni şifre ve tekrar alanlarını doldurmalısınız.");
                    return View(model);
                }
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                    return View(model);
                }
                if (model.NewPassword == model.CurrentPassword)
                {
                    ModelState.AddModelError("NewPassword", "Yeni şifre mevcut şifreyle aynı olamaz.");
                    return View(model);
                }
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("ConfirmNewPassword", "Yeni şifreler eşleşmiyor.");
                    return View(model);
                }

                user.Password = model.NewPassword;
            }

            user.Username = model.Username;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;

            _userService.Update(user);
            HttpContext.Session.SetString("Username", user.Username ?? "");
            TempData["Message"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ClearDebt(int userId)
        {
            try
            {
                var user = _userService.GetById(userId);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }
                user.Debt = 0;
                _userService.Update(user);
                TempData["Message"] = $"{user.Username} adlı kullanıcının borcu sıfırlandı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Borç sıfırlama başarısız: {ex.Message}";
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//kimlik doğrulama sonlandırılır.
            return RedirectToAction("Login");
        }
    }
}