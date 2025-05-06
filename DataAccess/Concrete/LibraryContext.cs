using Entity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.Book)
                .WithMany()
                .HasForeignKey(b => b.BookId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BookId);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Roman" },
                new Category { Id = 2, Name = "Bilim Kurgu" },
                new Category { Id = 3, Name = "Tarih" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Suç ve Ceza", Author = "Dostoyevski", IsAvailable = true, CategoryId = 1 },
                new Book { Id = 2, Title = "Dune", Author = "Frank Herbert", IsAvailable = true, CategoryId = 2 },
                new Book { Id = 3, Title = "Nutuk", Author = "Mustafa Kemal Atatürk", IsAvailable = true, CategoryId = 3 }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345"),
                    IsAdmin = true,
                    Name = "Admin",
                    Surname = "User",
                    Email = "admin@example.com",
                    IsEmailVerified = true,
                    PasswordResetToken = null,
                    PasswordResetTokenExpiry = null
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345"),
                    IsAdmin = false,
                    Name = "User",
                    Surname = "One",
                    Email = "user1@example.com",
                    IsEmailVerified = true,
                    PasswordResetToken = null,
                    PasswordResetTokenExpiry = null
                }
            );
            modelBuilder.Entity<Borrow>().HasData(
                new Borrow { Id = 1, UserId = 2, BookId = 1, BorrowDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(-3), ReturnDate = null }
            );

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
