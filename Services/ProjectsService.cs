using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Requests;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System;
using System.Linq;

namespace BugTrackingSystem.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly BugTrackingDbContext _context;

        public ProjectsService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult GetProjects()
        {
            var projects = _context.Projects
                .Where(p => !p.IsDeleted)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    CreatedByName = p.CreatedByUser.FullName,
                    UpdatedAt = p.UpdatedAt,
                    UpdatedByName = p.UpdatedByUser != null ? p.UpdatedByUser.FullName : null
                })
                .ToList();

            return ServiceResult.Ok(projects);
        }

        public ServiceResult CreateProject(CreateProjectRequest request, int currentUserId)
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "Active",
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            return ServiceResult.Ok(new { ProjectId = project.Id }, "Project created successfully.");
        }

        public ServiceResult UpdateProject(int id, UpdateProjectRequest request, int currentUserId)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (project == null)
                return ServiceResult.Fail("Project not found.", ServiceStatus.NotFound);

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.Status = string.IsNullOrWhiteSpace(request.Status) ? project.Status : request.Status;
            project.UpdatedBy = currentUserId;
            project.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Project updated successfully.");
        }

        public ServiceResult DeleteProject(int id, int currentUserId)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (project == null)
                return ServiceResult.Fail("Project not found.", ServiceStatus.NotFound);

            project.IsDeleted = true;
            project.DeletedBy = currentUserId;
            project.DeletedAt = DateTime.Now;

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Project deleted successfully.");
        }
    }
}
