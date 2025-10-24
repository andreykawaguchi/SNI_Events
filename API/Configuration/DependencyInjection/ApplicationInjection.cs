using SNI_Events.Application.Services;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Services;
using SNI_Events.Application.UseCases.User;
using SNI_Events.Application.UseCases.UserDinner;

namespace SNI_Events.API.Configuration.DependencyInjection
{
    public static class ApplicationInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDinnerService, DinnerService>();

            // Domain Services
            services.AddScoped<IUserDomainService, UserDomainService>();
            services.AddScoped<IUserDinnerDomainService, UserDinnerDomainService>();

            // User Use Cases
            services.AddScoped<CreateUserUseCase>();
            services.AddScoped<UpdateUserUseCase>();
            services.AddScoped<ChangePasswordUseCase>();
            services.AddScoped<DeleteUserUseCase>();
            services.AddScoped<GetUserByIdUseCase>();
            services.AddScoped<GetPagedUsersUseCase>();

            // UserDinner Use Cases
            services.AddScoped<AddUserToDinnerUseCase>();
            services.AddScoped<UpdateUserDinnerPaymentStatusUseCase>();
            services.AddScoped<UpdateUserDinnerPresenceUseCase>();

            return services;
        }
    }
}
