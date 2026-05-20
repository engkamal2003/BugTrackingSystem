using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly BugTrackingDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(BugTrackingDbContext context)
        {
            _context = context;
            _jwtService = new JwtService();
        }

        public ServiceResult Login(string email, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.PasswordHash == password &&
                    !u.IsDeleted);

            if (user == null)
                return ServiceResult.Fail("Invalid email or password.");

            user.LastLoginAt = DateTime.Now;
            _context.SaveChanges();

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                MustChangePassword = user.MustChangePassword,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted
            };

            if (user.MustChangePassword)
            {
                return ServiceResult.Ok(new LoginResponseDto
                {
                    MustChangePassword = true,
                    Token = null,
                    User = userDto
                }, "You must change your password before continuing.");
            }

            List<string> permissions;
            try
            {
                permissions = _context.RolePermissions
                    .Where(rp => rp.RoleId == user.RoleId)
                    .Select(rp => rp.Permission.Name)
                    .ToList();
            }
            catch
            {
                permissions = new List<string>();
            }

            return ServiceResult.Ok(new LoginResponseDto
            {
                MustChangePassword = false,
                Token = _jwtService.GenerateToken(user, permissions),
                User = userDto
            }, "Login successful.");
        }

        public ServiceResult ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Id == userId &&
                u.PasswordHash == oldPassword &&
                !u.IsDeleted);

            if (user == null)
                return ServiceResult.Fail("Invalid user or old password.");

            user.PasswordHash = newPassword;
            user.MustChangePassword = false;
            user.PasswordChangedAt = DateTime.Now;

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Password changed successfully. You can now login normally.");
        }
    }
}
