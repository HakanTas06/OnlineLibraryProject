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
    options.IdleTimeout = TimeSpan.FromMinutes(30);//Oturum 30 dakika hareketsiz kal�rsa sona erer.
    options.Cookie.HttpOnly = true;//g�venlik i�in
    options.Cookie.IsEssential = true;
});

//Cookie tabanl� kimlik do�rulama i�in bir �ema tan�mlar.Her sayfa y�klendi�inde giri� yapmaya gerek kalmaz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // oturum a�mam��sa logine y�nlendirir
        options.LogoutPath = "/Account/Logout";//��k�� i�in
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie s�resi
        options.SlidingExpiration = true; // Aktiviteyle s�re uzar
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;//zorunlu �erez
        options.Cookie.Expiration = null; // ��k�� yapmadan silince oturum oto kapat�l�r.
    });

var app = builder.Build();
app.UseStaticFiles();// Statik dosyalar i�in
app.UseRouting();
app.UseAuthentication(); // Kimlik do�rulama middleware�ini ekler.
app.UseAuthorization();  // Yetkilendirme middleware�ini ekler.
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();