namespace SNI_Events.API.Configuration.DependencyInjection
{
    public static class AutoMapperConfiguration
    {
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            // Carrega todos os perfis dentro dos assemblies do projeto
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
