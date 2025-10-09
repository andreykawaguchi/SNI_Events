namespace SNI_Events.API.Configuration.DependencyInjection
{
    public static class DomainInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // Ex: services.AddScoped<IEmailValidator, EmailValidator>();
            return services;
        }
    }
}
