using Microsoft.OpenApi.Models;

namespace SNI_Events.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SNI Events API",
                    Version = "v1"
                });
                // Adicione autenticação no Swagger aqui, se desejar
            });

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SNI Events API v1"));
            return app;
        }
    }
}
