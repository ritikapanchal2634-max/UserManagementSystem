using System.Security.Claims;
using UserManagementSystem.Models.Entities;

namespace UserManagementSystem.IServices
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
    }
}
