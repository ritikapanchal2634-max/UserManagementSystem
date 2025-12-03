using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.IServices;
using UserManagementSystem.Models.Entities;
using UserManagementSystem.Models.ViewModels;
using static UserManagementSystem.Services.UserService;

namespace UserManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model, List<string> documentPaths)
        {
            var user = new User
            {
                Name = model.Name,
                UserName = model.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Hobbies = model.Hobbies != null ? string.Join(",", model.Hobbies) : "",
                Address = model.Address,
                StateId = model.StateId,
                CityId = model.CityId,
                Pincode = model.Pincode,
                Role = "User", // Default role
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Add documents
            if (documentPaths != null && documentPaths.Any())
            {
                foreach (var path in documentPaths)
                {
                    var fileName = System.IO.Path.GetFileName(path);
                    var fileInfo = new System.IO.FileInfo(path);

                    var document = new UserDocument
                    {
                        UserId = user.Id,
                        FileName = fileName,
                        FilePath = path,
                        FileType = fileInfo.Extension,
                        FileSize = fileInfo.Length,
                        UploadedDate = DateTime.UtcNow
                    };

                    _context.UserDocuments.Add(document);
                }

                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.State)
                .Include(u => u.City)
                .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);

            if (user == null)
                return null;

            var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task<PaginatedUserListViewModel> GetUsersAsync(int page, int pageSize, string sortBy, string sortOrder, string searchTerm)
        {
            var query = _context.Users
                .Include(u => u.State)
                .Include(u => u.City)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.Name.Contains(searchTerm) ||
                    u.UserName.Contains(searchTerm) ||
                    u.State.Name.Contains(searchTerm) ||
                    u.City.Name.Contains(searchTerm));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "name" => sortOrder == "desc" ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "username" => sortOrder == "desc" ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
                "dateofbirth" => sortOrder == "desc" ? query.OrderByDescending(u => u.DateOfBirth) : query.OrderBy(u => u.DateOfBirth),
                "createddate" => sortOrder == "desc" ? query.OrderByDescending(u => u.CreatedDate) : query.OrderBy(u => u.CreatedDate),
                _ => query.OrderByDescending(u => u.CreatedDate)
            };

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var users = await query

                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    State = u.State.Name,
                    City = u.City.Name,
                    CreatedDate = u.CreatedDate,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return new PaginatedUserListViewModel
            {
                Users = users,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                SortBy = sortBy,
                SortOrder = sortOrder,
                SearchTerm = searchTerm
            };
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.State)
                .Include(u => u.City)
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateUserAsync(EditUserViewModel model, List<string> newDocumentPaths)
        {
            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
                return false;

            user.Name = model.Name;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.Hobbies = model.Hobbies != null ? string.Join(",", model.Hobbies) : "";
            user.Address = model.Address;
            user.StateId = model.StateId;
            user.CityId = model.CityId;
            user.Pincode = model.Pincode;
            user.ModifiedDate = DateTime.UtcNow;

            // Add new documents
            if (newDocumentPaths != null && newDocumentPaths.Any())
            {
                foreach (var path in newDocumentPaths)
                {
                    var fileName = System.IO.Path.GetFileName(path);
                    var fileInfo = new System.IO.FileInfo(path);

                    var document = new UserDocument
                    {
                        UserId = user.Id,
                        FileName = fileName,
                        FilePath = path,
                        FileType = fileInfo.Extension,
                        FileSize = fileInfo.Length,
                        UploadedDate = DateTime.UtcNow
                    };

                    _context.UserDocuments.Add(document);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return false;

            // Delete associated documents from file system
            foreach (var doc in user.Documents)
            {
                if (System.IO.File.Exists(doc.FilePath))
                {
                    System.IO.File.Delete(doc.FilePath);
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<State>> GetStatesAsync()
        {
            return await _context.States.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<City>> GetCitiesByStateAsync(int stateId)
        {
            return await _context.Cities
                .Where(c => c.StateId == stateId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }

}
