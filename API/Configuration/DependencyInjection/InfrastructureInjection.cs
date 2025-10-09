using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Repositories.Base;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Services;
using SNI_Events.Infraestructure.Context;
using SNI_Events.Infraestructure.Repository;
using SNI_Events.Infraestructure.Repository.Base;

namespace SNI_Events.API.Configuration.DependencyInjection
{
    public static class InfrastructureInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
