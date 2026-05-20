using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class UsersService : IUsersService
    {
        private readonly BugTrackingDbContext _context;

        public UsersService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    MustChangePassword = u.MustChangePassword,
                    LastLoginAt = u.LastLoginAt,
                    CreatedAt = u.CreatedAt,
                    IsDeleted = u.IsDeleted
                })
                .ToList();

            return ServiceResult.Ok(users);
        }

        public ServiceResult CreateUser(CreateUserRequest request, int currentUserId)
        {
            if (!_context.Roles.Any(r => r.Id == request.RoleId && !r.IsDeleted))
                return ServiceResult.Fail("Invalid role.");

            if (_context.Users.Any(u => u.Email == request.Email))
                return ServiceResult.Fail("Email already exists.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password,
                RoleId = request.RoleId,
                MustChangePassword = true,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return ServiceResult.Ok(new { UserId = user.Id }, "User created successfully.");
        }

        public ServiceResult UpdateUserRole(int id, int roleId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                return ServiceResult.Fail("User not found.", ServiceStatus.NotFound);

            if (!_context.Roles.Any(r => r.Id == roleId && !r.IsDeleted))
                return ServiceResult.Fail("Invalid role.");

            user.RoleId = roleId;
            _context.SaveChanges();

            return ServiceResult.Ok(message: "User role updated successfully.");
        }

        public ServiceResult UpdateUserStatus(int id, bool isActive, int currentUserId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return ServiceResult.Fail("User not found.", ServiceStatus.NotFound);

            if (!isActive)
            {
                user.IsDeleted = true;
                user.DeletedBy = currentUserId;
                user.DeletedAt = DateTime.Now;
            }
            else
            {
                user.IsDeleted = false;
                user.DeletedBy = null;
                user.DeletedAt = null;
            }

            _context.SaveChanges();

            return ServiceResult.Ok(message: "User status updated successfully.");
        }
    }
}
