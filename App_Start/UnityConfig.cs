using BugTrackingSystem.Data;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;
using System.Web.Http;

namespace BugTrackingSystem
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<BugTrackingDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IProjectsService, ProjectsService>();
            container.RegisterType<IBugsService, BugsService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<INotificationsService, NotificationsService>();
            container.RegisterType<IAttachmentsService, AttachmentsService>();
            container.RegisterType<IRolesService, RolesService>();
            container.RegisterType<IPermissionsService, PermissionsService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
