using LibraryD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MyDbContext _context;

        public ProfileController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users
                .Include(u => u.Borrowings).ThenInclude(b => b.Book)
                .Include(u => u.RoomReservations).ThenInclude(r => r.Room)
                .FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null) return NotFound();

            return View(user);
        }

        // تعديل بيانات المستخدم
        [HttpGet]
        public IActionResult Edit()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId.Value);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model)
        {
            var user = _context.Users.Find(model.UserId);
            if (user == null) return RedirectToAction("Index");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;

            // تم إزالة أي استخدام لـ Profile_Picture لأنه غير موجود في الموديل

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }

        // تغيير كلمة المرور
        [HttpPost]
        public IActionResult ResetPassword(string oldPassword, string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId.Value);
            if (user == null) return RedirectToAction("Login", "Account");

            if (user.Password != oldPassword) // بدون تشفير
            {
                TempData["Error"] = "Old password is incorrect.";
                return RedirectToAction("Index");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("Index");
            }

            user.Password = newPassword; // بدون تشفير
            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
