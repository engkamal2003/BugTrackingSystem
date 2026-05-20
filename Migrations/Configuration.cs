namespace BugTrackingSystem.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTrackingSystem.Data.BugTrackingDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BugTrackingSystem.Data.BugTrackingDbContext context)
        {
            // Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddOrUpdate(r => r.Name,
                    new Models.Role { Name = "SystemAdmin" },
                    new Models.Role { Name = "ProjectAdmin" },
                    new Models.Role { Name = "DevelopmentManager" },
                    new Models.Role { Name = "TestingManager" },
                    new Models.Role { Name = "Developer" },
                    new Models.Role { Name = "Tester" },
                    new Models.Role { Name = "ProjectManager" }
                );
                context.SaveChanges();
            }

            // Admin user
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.First(r => r.Name == "SystemAdmin");
                context.Users.AddOrUpdate(u => u.Email,
                    new Models.User
                    {
                        FullName = "System Admin",
                        Email = "admin@test.com",
                        PasswordHash = "Admin@123",
                        RoleId = adminRole.Id,
                        IsDeleted = false,
                        MustChangePassword = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // Permissions
            SeedPermissions(context);

            // Role-Permission mappings
            SeedRolePermissions(context);
        }

        private void SeedPermissions(BugTrackingSystem.Data.BugTrackingDbContext context)
        {
            var permissions = new[]
            {
                new Models.Permission { Name = "ViewUsers",            Description = "View all users",                   Group = "Users" },
                new Models.Permission { Name = "CreateUser",           Description = "Create a new user",                Group = "Users" },
                new Models.Permission { Name = "UpdateUserRole",       Description = "Change a user's role",             Group = "Users" },
                new Models.Permission { Name = "UpdateUserStatus",     Description = "Activate or deactivate a user",    Group = "Users" },

                new Models.Permission { Name = "ViewProjects",         Description = "View all projects",                Group = "Projects" },
                new Models.Permission { Name = "CreateProject",        Description = "Create a new project",             Group = "Projects" },
                new Models.Permission { Name = "UpdateProject",        Description = "Edit an existing project",         Group = "Projects" },
                new Models.Permission { Name = "DeleteProject",        Description = "Delete a project",                 Group = "Projects" },

                new Models.Permission { Name = "ViewBugs",             Description = "View all bugs",                    Group = "Bugs" },
                new Models.Permission { Name = "CreateBug",            Description = "Report a new bug",                 Group = "Bugs" },
                new Models.Permission { Name = "UpdateBugStatus",      Description = "Change the status of a bug",       Group = "Bugs" },
                new Models.Permission { Name = "RetestBug",            Description = "Mark a bug for retest",            Group = "Bugs" },

                new Models.Permission { Name = "ViewNotifications",    Description = "View notifications",               Group = "Notifications" },
                new Models.Permission { Name = "MarkNotificationRead", Description = "Mark a notification as read",      Group = "Notifications" },

                new Models.Permission { Name = "ViewAttachments",      Description = "View and download attachments",    Group = "Attachments" },
                new Models.Permission { Name = "UploadAttachment",     Description = "Upload a file attachment",         Group = "Attachments" },
                new Models.Permission { Name = "DeleteAttachment",     Description = "Delete an attachment",             Group = "Attachments" },

                new Models.Permission { Name = "ViewDashboard",        Description = "View the dashboard summary",       Group = "Dashboard" },

                new Models.Permission { Name = "ViewPermissions",      Description = "View role permissions",            Group = "Permissions" },
                new Models.Permission { Name = "ManagePermissions",    Description = "Assign permissions to roles",      Group = "Permissions" },
            };

            foreach (var perm in permissions)
            {
                if (!context.Permissions.Any(p => p.Name == perm.Name))
                    context.Permissions.Add(perm);
            }

            context.SaveChanges();
        }

        private void SeedRolePermissions(BugTrackingSystem.Data.BugTrackingDbContext context)
        {
            var allPerms = context.Permissions.ToDictionary(p => p.Name, p => p.Id);

            var rolePermMap = new Dictionary<string, string[]>
            {
                ["SystemAdmin"] = new[]
                {
                    "ViewUsers", "CreateUser", "UpdateUserRole", "UpdateUserStatus",
                    "ViewProjects", "CreateProject", "UpdateProject", "DeleteProject",
                    "ViewBugs", "CreateBug", "UpdateBugStatus", "RetestBug",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment", "DeleteAttachment",
                    "ViewDashboard",
                    "ViewPermissions", "ManagePermissions"
                },
                ["ProjectAdmin"] = new[]
                {
                    "ViewUsers",
                    "ViewProjects", "CreateProject", "UpdateProject", "DeleteProject",
                    "ViewBugs",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment",
                    "ViewDashboard"
                },
                ["DevelopmentManager"] = new[]
                {
                    "ViewProjects",
                    "ViewBugs", "UpdateBugStatus",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment",
                    "ViewDashboard"
                },
                ["TestingManager"] = new[]
                {
                    "ViewProjects",
                    "ViewBugs", "CreateBug", "RetestBug",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment", "DeleteAttachment",
                    "ViewDashboard"
                },
                ["Developer"] = new[]
                {
                    "ViewProjects",
                    "ViewBugs", "UpdateBugStatus",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment",
                    "ViewDashboard"
                },
                ["Tester"] = new[]
                {
                    "ViewProjects",
                    "ViewBugs", "CreateBug", "RetestBug",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments", "UploadAttachment",
                    "ViewDashboard"
                },
                ["ProjectManager"] = new[]
                {
                    "ViewProjects",
                    "ViewBugs",
                    "ViewNotifications", "MarkNotificationRead",
                    "ViewAttachments",
                    "ViewDashboard"
                }
            };

            foreach (var kvp in rolePermMap)
            {
                var role = context.Roles.FirstOrDefault(r => r.Name == kvp.Key);
                if (role == null) continue;

                foreach (var permName in kvp.Value)
                {
                    if (!allPerms.TryGetValue(permName, out int permId)) continue;

                    var exists = context.RolePermissions
                        .Any(rp => rp.RoleId == role.Id && rp.PermissionId == permId);

                    if (!exists)
                    {
                        context.RolePermissions.Add(new Models.RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permId
                        });
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
