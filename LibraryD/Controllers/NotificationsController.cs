using LibraryD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryD.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MyDbContext _context;

        public NotificationsController(MyDbContext context)
        {
            _context = context;
        }

        // عرض كل الإشعارات للمستخدم
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notifications = _context.Notifications
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.Date)
                .ToList();

            // حساب عدد الإشعارات غير المقروءة
            ViewBag.UnreadCount = notifications.Count(n => n.IsRead == false);

            return View(notifications);
        }

        // وضع إشعار كمقروء
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
                TempData["Success"] = "Notification marked as read.";
            }

            return RedirectToAction("Index");
        }

        // وضع كل الإشعارات كمقروءة
        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var unreadNotifications = _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead == false);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            _context.SaveChanges();
            TempData["Success"] = "All notifications marked as read.";

            return RedirectToAction("Index");
        }

        // حذف إشعار
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
                TempData["Success"] = "Notification deleted.";
            }

            return RedirectToAction("Index");
        }

        // حذف كل الإشعارات
        [HttpPost]
        public IActionResult DeleteAll()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var notifications = _context.Notifications
                .Where(n => n.UserId == userId);

            _context.Notifications.RemoveRange(notifications);
            _context.SaveChanges();

            TempData["Success"] = "All notifications deleted.";
            return RedirectToAction("Index");
        }

        // الحصول على عدد الإشعارات غير المقروءة (للاستخدام في AJAX)
        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { count = 0 });

            var count = _context.Notifications
                .Count(n => n.UserId == userId && n.IsRead == false);

            return Json(new { count });
        }
    }
}