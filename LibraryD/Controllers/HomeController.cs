using LibraryD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Controllers;

public class HomeController : Controller
{
    private readonly MyDbContext _context;
    public HomeController(MyDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search, int? categoryId)
    {
        var books = _context.Books
            .Include(b => b.Status)
            .Include(b => b.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            books = books.Where(b => b.BookName.Contains(search));

        if (categoryId != null)
            books = books.Where(b => b.CategoryId == categoryId);

        ViewBag.Categories = _context.Category.ToList();
        ViewBag.Search = search;
        ViewBag.SelectedCategory = categoryId;

        return View(books.ToList());
    }

    [HttpPost]
    public IActionResult Borrow(int id)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        bool isBlacklisted = _context.Blacklists.Any(b => b.UserId == userId);
        if (isBlacklisted)
        {
            TempData["Error"] = "You are blacklisted.";
            return RedirectToAction("Index");
        }

        var book = _context.Books.FirstOrDefault(b => b.BookId == id);
        if (book == null)
        {
            TempData["Error"] = "Book not found.";
            return RedirectToAction("Index");
        }

        if (book.StatusId != 1)
        {
            TempData["Error"] = "Book is not available.";
            return RedirectToAction("Index");
        }

        bool alreadyRequested = _context.Borrowings.Any(b =>
            b.BookId == id && b.UserId == userId && (b.StatusId == 2 || b.StatusId == 4));

        if (alreadyRequested)
        {
            TempData["Error"] = "You already requested this book.";
            return RedirectToAction("Index");
        }

        Borrowing borrowing = new Borrowing
        {
            BookId = id,
            UserId = userId.Value,
            BorrowDate = DateTime.Now,
            StatusId = 2
        };
          
        _context.Borrowings.Add(borrowing);

        Notification notification = new Notification
        {
            UserId = userId.Value,
            Message = "Your borrow request has been sent.",
            Date = DateTime.Now,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        _context.SaveChanges();

        TempData["Success"] = "Request sent successfully.";
        return RedirectToAction("Index");
    }
}
