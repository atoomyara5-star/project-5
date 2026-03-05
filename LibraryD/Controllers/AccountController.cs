using LibraryD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryD.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(MyDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ================================
        // Login
        // ================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "❌ Please enter email and password";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "❌ Email not found! Please check your email";
                return View();
            }

            if (!_passwordHasher.Verify(password, user.Password))
            {
                TempData["Error"] = "❌ Incorrect password! Please try again";
                return View();
            }

            // تخزين البيانات في السيشن
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("Role", user.Role ?? "Student");

            TempData["Success"] = $"✅ Welcome back, {user.FirstName}!";

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // ================================
        // Logout
        // ================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "✅ You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // ================================
        // Sign Up
        // ================================
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User model, string confirmPassword)
        {
            // التحقق من البريد الإلكتروني الجامعي
            if (!model.Email.EndsWith("@ses.yu.edu.jo"))
            {
                ModelState.AddModelError("Email", "❌ Email must be a university email (@ses.yu.edu.jo)");
            }

            // التحقق من تطابق كلمة المرور
            if (model.Password != confirmPassword)
            {
                ModelState.AddModelError("Password", "❌ Passwords do not match");
            }

            // التحقق من عدم وجود البريد الإلكتروني مسبقاً
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "❌ Email already registered");
            }

            if (ModelState.IsValid)
            {
                model.Role = "Student";
                model.Password = _passwordHasher.Hash(model.Password);

                _context.Users.Add(model);
                _context.SaveChanges();

                // تسجيل الدخول مباشرة بعد التسجيل
                HttpContext.Session.SetInt32("UserId", model.UserId);
                HttpContext.Session.SetString("UserName", model.FirstName + " " + model.LastName);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("Role", model.Role);

                TempData["Success"] = $"✅ Welcome, {model.FirstName}! Your account has been created.";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // ================================
        // Forgot Password
        // ================================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "❌ Please enter your email";
                return View();
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                TempData["Error"] = "❌ Please enter new password";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "❌ Passwords do not match";
                return View();
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "❌ Password must be at least 6 characters";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "❌ Email not found!";
                return View();
            }

            // تحديث كلمة المرور
            user.Password = _passwordHasher.Hash(newPassword);
            _context.SaveChanges();

            TempData["Success"] = "✅ Password reset successfully! You can now login with your new password.";
            return RedirectToAction("Login");
        }
    }
}