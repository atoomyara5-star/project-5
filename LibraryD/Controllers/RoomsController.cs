using LibraryD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryD.Controllers
{
    public class RoomsController : Controller
    {
        private readonly MyDbContext _context;

        public RoomsController(MyDbContext context)
        {
            _context = context;
        }

        // ===============================
        // عرض جميع الغرف
        // ===============================
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var rooms = _context.Rooms
                .Include(r => r.Status)
                .ToList();

            return View(rooms);
        }

        // ===============================
        // حجز غرفة
        // ===============================
        [HttpPost]
        public IActionResult Reserve(int roomId, DateTime reservationDate, TimeSpan startTime)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // تحويل DateTime إلى DateOnly
            DateOnly resDate = DateOnly.FromDateTime(reservationDate);

            // التحقق من أن التاريخ ليس في الماضي
            if (resDate < DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["Error"] = "Cannot reserve a room in the past.";
                return RedirectToAction("Index");
            }

            // التحقق من أن الوقت ضمن ساعات العمل (8 صباحاً - 8 مساءً)
            if (startTime.Hours < 8 || startTime.Hours >= 20)
            {
                TempData["Error"] = "Reservations are only allowed between 8:00 AM and 8:00 PM.";
                return RedirectToAction("Index");
            }

            // التحقق من البلاك ليست
            bool isBlacklisted = _context.Blacklists
                .Any(b => b.UserId == userId.Value);

            if (isBlacklisted)
            {
                TempData["Error"] = "You are blacklisted and cannot reserve rooms.";
                return RedirectToAction("Index");
            }

            // التحقق من عدم وجود حجز مكرر لنفس الغرفة في نفس الوقت
            TimeOnly start = TimeOnly.FromTimeSpan(startTime);
            TimeOnly end = start.AddHours(2);

            bool existingReservation = _context.RoomReservations
                .Any(r => r.RoomId == roomId &&
                         r.ReservationDate == resDate &&
                         ((r.StartTime <= start && r.EndTime > start) ||
                          (r.StartTime < end && r.EndTime >= end)) &&
                         (r.StatusId == 3 || r.StatusId == 4)); // Pending or Confirmed

            if (existingReservation)
            {
                TempData["Error"] = "This room is already reserved at the selected time.";
                return RedirectToAction("Index");
            }

            // إنشاء الحجز - بحالة Pending (3)
            RoomReservation reservation = new RoomReservation
            {
                RoomId = roomId,
                UserId = userId.Value,
                ReservationDate = resDate,
                StartTime = start,
                StatusId = 3 // Pending
            };

            _context.RoomReservations.Add(reservation);
            _context.SaveChanges();

            // إرسال إشعار للأدمن
            var admins = _context.Users.Where(u => u.Role == "Admin").ToList();
            foreach (var admin in admins)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = admin.UserId,
                    Message = $"New room reservation request from {user.FirstName} {user.LastName} for Room {roomId} on {reservationDate:yyyy-MM-dd} at {startTime}",
                    IsRead = false,
                    Date = DateTime.Now
                });
            }
            _context.SaveChanges();

            TempData["Success"] = "Room reservation request submitted successfully. Waiting for admin approval.";
            return RedirectToAction("MyReservations");
        }

        // ===============================
        // عرض حجوزات المستخدم
        // ===============================
        public IActionResult MyReservations()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var myReservations = _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Status)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.ReservationDate)
                .ThenBy(r => r.StartTime)
                .ToList();

            return View(myReservations);
        }

        // ===============================
        // إلغاء الحجز (للمستخدم)
        // ===============================
        [HttpPost]
        public IActionResult CancelReservation(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var reservation = _context.RoomReservations
                .Include(r => r.Room)
                .FirstOrDefault(r => r.ReservationId == id && r.UserId == userId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("MyReservations");
            }

            // يمكن إلغاء الحجوزات المعلقة فقط
            if (reservation.StatusId == 3) // Pending
            {
                _context.RoomReservations.Remove(reservation);
                _context.SaveChanges();
                TempData["Success"] = "Reservation cancelled successfully.";
            }
            else if (reservation.StatusId == 4) // Confirmed
            {
                // يمكن إلغاء الحجوزات المؤكدة قبل 24 ساعة
                var reservationDateTime = reservation.ReservationDate.ToDateTime(TimeOnly.MinValue).Add(reservation.StartTime.ToTimeSpan());
                if (reservationDateTime > DateTime.Now.AddHours(24))
                {
                    reservation.StatusId = 5; // Cancelled
                    _context.SaveChanges();
                    TempData["Success"] = "Reservation cancelled successfully.";
                }
                else
                {
                    TempData["Error"] = "Confirmed reservations can only be cancelled at least 24 hours in advance.";
                }
            }
            else
            {
                TempData["Error"] = "This reservation cannot be cancelled.";
            }

            return RedirectToAction("MyReservations");
        }
    }
} 