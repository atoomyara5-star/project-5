using LibraryD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryD.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly MyDbContext _context;

        public FeedbackController(MyDbContext context)
        {
            _context = context;
        }

        // =========================================
        // الطالب - عرض صفحة إرسال الفيدباك
        // =========================================
        [HttpGet]
        public IActionResult Submit()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // تخزين الايميل في session للعرض
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                HttpContext.Session.SetString("Email", user.Email);
            }

            return View();
        }

        // =========================================
        // الطالب - إرسال الفيدباك
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(string message)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty!";
                return RedirectToAction("Submit");
            }

            Feedback feedback = new Feedback
            {
                UserId = userId.Value,
                Message = message.Trim(),
                Date = DateTime.Now
            };

            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();

            // تحديث عدد الفيدباك للأدمن
            int count = _context.Feedbacks.Count();
            HttpContext.Session.SetInt32("FeedbackCount", count);

            TempData["Success"] = "Feedback sent successfully!";
            return RedirectToAction("Submit");
        }

        // =========================================
        // الأدمن - عرض صفحة الفيدباك (بنفس اسم الـ View)
        // =========================================
        public IActionResult Feedback()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Submit");

            var feedbacks = _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.Date)
                .ToList();

            // تحديث العداد في السايدبار
            HttpContext.Session.SetInt32("FeedbackCount", feedbacks.Count);

            return View("Index", feedbacks);
        }

        // =========================================
        // الأدمن - عرض كل الفيدباك (للتوافق مع الكود القديم)
        // =========================================
        public IActionResult Index()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Submit");

            var feedbacks = _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.Date)
                .ToList();

            // تحديث العداد في السايدبار
            HttpContext.Session.SetInt32("FeedbackCount", feedbacks.Count);

            return View(feedbacks);
        }

        // =========================================
        // الأدمن - حذف فيدباك
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Submit");

            var feedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);

            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }

            // تحديث العداد بعد الحذف
            int count = _context.Feedbacks.Count();
            HttpContext.Session.SetInt32("FeedbackCount", count);

            return RedirectToAction("Feedback");
        }
    }
}