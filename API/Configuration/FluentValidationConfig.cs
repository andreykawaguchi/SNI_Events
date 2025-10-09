using FluentValidation;
using FluentValidation.AspNetCore;
using SNI_Events.API.Validators.User;

namespace SNI_Events.API.Configuration
{
    public static class FluentValidationConfig
    {
        public static IServiceCollection AddFluentValidationCustom(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<UserCreateRequestValidator>();
            return services;
        }
    }
}
