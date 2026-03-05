using LibraryD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Controllers
{
    public class BooksController : Controller
    {
        private readonly MyDbContext _context;

        public BooksController(MyDbContext context)
        {
            _context = context;
        }

        // =====================================
        // Show Books (6 per page + Search)
        // =====================================
        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 6; // عدد الكتب في كل صفحة

            var booksQuery = _context.Books
                .Include(b => b.Status)
                .Include(b => b.Category)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                booksQuery = booksQuery
                    .Where(b => b.BookName.Contains(search));
            }

            int totalBooks = booksQuery.Count();

            var books = booksQuery
                .OrderBy(b => b.BookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            ViewBag.Search = search;

            return View(books);
        }

        // =====================================
        // Borrow Book
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow(int bookId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);

            if (book == null || book.StatusId != 1) // 1 = Available
            {
                TempData["Error"] = "Book not available";
                return RedirectToAction("Index");
            }

            // منع تكرار الطلب
            bool alreadyRequested = _context.Borrowings.Any(b =>
                b.BookId == bookId &&
                b.UserId == userId &&
                (b.StatusId == 2 || b.StatusId == 3)); // Borrowed or Pending

            if (alreadyRequested)
            {
                TempData["Error"] = "You already requested this book";
                return RedirectToAction("Index");
            }

            var borrowing = new Borrowing
            {
                BookId = bookId,
                UserId = userId.Value,
                BorrowDate = DateTime.Now,
                StatusId = 3 // Pending
            };

            _context.Borrowings.Add(borrowing);
            _context.SaveChanges();

            TempData["Success"] = "Borrow request sent successfully";
            return RedirectToAction("Index");
        }

        // =====================================
        // Return Book
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(int borrowId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var borrow = _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefault(b =>
                    b.BorrowId == borrowId &&
                    b.UserId == userId &&
                    b.StatusId == 2); // Approved

            if (borrow != null)
            {
                borrow.StatusId = 4; // Returned
                borrow.ReturnDate = DateTime.Now;

                if (borrow.Book != null)
                {
                    borrow.Book.StatusId = 1; // Available
                }

                _context.SaveChanges();
                TempData["Success"] = "Book returned successfully";
            }
            else
            {
                TempData["Error"] = "Return failed";
            }

            return RedirectToAction("Index");
        }
    }
}