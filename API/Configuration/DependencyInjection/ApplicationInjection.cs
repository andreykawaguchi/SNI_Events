using SNI_Events.Application.Services;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Services;

namespace SNI_Events.API.Configuration.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();

            // Adicione aqui outros serviços de aplicação  

            return services;
        }
    }
}
