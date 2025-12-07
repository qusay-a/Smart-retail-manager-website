using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            // Uses DbSet<UserLogin> UserLogins
            var user = await _db.UserLogins
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login.");
                return View();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Email", user.Email)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserLogin());
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(UserLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var exists = await _db.UserLogins
                .AnyAsync(u => u.Username == model.Username || u.Email == model.Email);

            if (exists)
            {
                ModelState.AddModelError("", "Username or email already exists.");
                return View(model);
            }

            model.CreatedAt = DateTime.Now;

            _db.UserLogins.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
