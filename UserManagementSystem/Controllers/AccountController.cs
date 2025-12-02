using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Models.ViewModels;
using UserManagementSystem.Services;

namespace UserManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IFileUploadService _fileUploadService;

        public AccountController(IUserService userService, IJwtService jwtService, IFileUploadService fileUploadService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.States = await _userService.GetStatesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Server-side validation for date of birth
                if (model.DateOfBirth >= DateTime.Now.Date)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of birth cannot be today or in the future");
                    ViewBag.States = await _userService.GetStatesAsync();
                    return View(model);
                }

                // Check if username already exists
                if (await _userService.UserExistsAsync(model.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                    ViewBag.States = await _userService.GetStatesAsync();
                    return View(model);
                }

                // Upload files
                var (success, filePaths, errorMessage) = await _fileUploadService.UploadFilesAsync(model.Documents);
                if (!success)
                {
                    ModelState.AddModelError("Documents", errorMessage);
                    ViewBag.States = await _userService.GetStatesAsync();
                    return View(model);
                }

                // Register user
                var user = await _userService.RegisterAsync(model, filePaths);

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ViewBag.States = await _userService.GetStatesAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to user list
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateAsync(model.UserName, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);

                // Store token in cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = model.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                // Store user info in session
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Role", user.Role);

                TempData["SuccessMessage"] = $"Welcome back, {user.Name}!";
                return RedirectToAction("Index", "User");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            // Remove authentication cookie
            Response.Cookies.Delete("AuthToken");

            // Clear session
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var cities = await _userService.GetCitiesByStateAsync(stateId);
            return Json(cities);
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var exists = await _userService.UserExistsAsync(username);
            return Json(!exists);
        }

    }
}
