using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebLayer.EmailServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookService, BookManager>();
builder.Services.AddScoped<IBookDal, EfBookDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();
builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IBorrowService, BorrowManager>();
builder.Services.AddScoped<IBorrowDal, EfBorrowDal>();
builder.Services.AddScoped<ICommentService, CommentManager>();
builder.Services.AddScoped<ICommentDal, EfCommentDal>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);//Oturum 30 dakika hareketsiz kalýrsa sona erer.
    options.Cookie.HttpOnly = true;//güvenlik için
    options.Cookie.IsEssential = true;
});

//Cookie tabanlý kimlik doðrulama için bir þema tanýmlar.Her sayfa yüklendiðinde giriþ yapmaya gerek kalmaz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // oturum açmamýþsa logine yönlendirir
        options.LogoutPath = "/Account/Logout";//çýkýþ için
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie süresi
        options.SlidingExpiration = true; // Aktiviteyle süre uzar
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;//zorunlu çerez
        options.Cookie.Expiration = null; // çýkýþ yapmadan silince oturum oto kapatýlýr.
    });

var app = builder.Build();
app.UseStaticFiles();// Statik dosyalar için
app.UseRouting();
app.UseAuthentication(); // Kimlik doðrulama middleware’ini ekler.
app.UseAuthorization();  // Yetkilendirme middleware’ini ekler.
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();