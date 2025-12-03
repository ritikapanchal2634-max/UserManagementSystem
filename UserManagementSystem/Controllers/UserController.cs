using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Data;
using UserManagementSystem.IServices;
using UserManagementSystem.Models.ViewModels;
using UserManagementSystem.Services;

namespace UserManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;

        public UserController(IUserService userService, IFileUploadService fileUploadService, ApplicationDbContext context)
        {
            _userService = userService;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string sortBy = "CreatedDate",string sortOrder = "desc", string searchTerm = "")
        {
            var result = await _userService.GetUsersAsync(page, pageSize, sortBy, sortOrder, searchTerm);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Index");
            }

            // Check if current user has permission to edit
            var currentUserName = HttpContext.Session.GetString("UserName");
            var currentUserRole = HttpContext.Session.GetString("Role");

            if (currentUserRole != "Admin" && user.UserName != currentUserName)
            {
                TempData["ErrorMessage"] = "You don't have permission to edit this user";
                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Hobbies = user.Hobbies?.Split(',').ToList(),
                Address = user.Address,
                StateId = user.StateId,
                CityId = user.CityId,
                Pincode = user.Pincode,
                ExistingDocuments = user.Documents?.Select(d => new UserDocumentViewModel
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    FileType = d.FileType,
                    FileSize = d.FileSize,
                    UploadedDate = d.UploadedDate
                }).ToList()
            };

            ViewBag.States = await _userService.GetStatesAsync();
            ViewBag.Cities = await _userService.GetCitiesByStateAsync(user.StateId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Server-side validation for date of birth
                if (model.DateOfBirth >= DateTime.Now.Date)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of birth cannot be today or in the future");
                    ViewBag.States = await _userService.GetStatesAsync();
                    ViewBag.Cities = await _userService.GetCitiesByStateAsync(model.StateId);
                    return View(model);
                }

                // Get existing documents from database
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user != null)
                {
                    // Convert existing documents to view model
                    model.ExistingDocuments = user.Documents?.Select(d => new UserDocumentViewModel
                    {
                        Id = d.Id,
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        FileType = d.FileType,
                        FileSize = d.FileSize,
                        UploadedDate = d.UploadedDate
                    }).ToList();
                }

                // Upload new files if any
                List<string> filePaths = null;
                if (model.NewDocuments != null && model.NewDocuments.Any())
                {
                    var (success, paths, errorMessage) = await _fileUploadService.UploadFilesAsync(model.NewDocuments);
                    if (!success)
                    {
                        ModelState.AddModelError("NewDocuments", errorMessage);
                        ViewBag.States = await _userService.GetStatesAsync();
                        ViewBag.Cities = await _userService.GetCitiesByStateAsync(model.StateId);
                        return View(model);
                    }
                    filePaths = paths;
                }

                var result = await _userService.UpdateUserAsync(model, filePaths);
                if (result)
                {
                    return RedirectToAction("Index", new { message = "User updated successfully", type = "success" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Failed to update user", type = "error" });
                }

            }

            ViewBag.States = await _userService.GetStatesAsync();
            ViewBag.Cities = await _userService.GetCitiesByStateAsync(model.StateId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var document = await _context.UserDocuments.FindAsync(documentId);
            if (document == null || !System.IO.File.Exists(document.FilePath))
            {
                TempData["ErrorMessage"] = "Document not found";
                return RedirectToAction("Index");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
            return File(fileBytes, "application/octet-stream", document.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int documentId, int userId)
        {
            var document = await _context.UserDocuments.FindAsync(documentId);
            if (document == null)
            {
                return Json(new { success = false, message = "Document not found" });
            }

            // Delete file from file system
            _fileUploadService.DeleteFile(document.FilePath);

            // Delete from database
            _context.UserDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Document deleted successfully" });
        }
    }
}
