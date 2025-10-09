using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SNI_Events.API.Configuration;
using SNI_Events.API.Configuration.DependencyInjection;
using SNI_Events.Infraestructure.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SNIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddFluentValidationCustom();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();
builder.Services.AddDomainServices();

builder.Services.AddAutoMapperConfiguration();

#region Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SNI_Events API", Version = "v1" });

    // 🔐 Define o esquema de segurança
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo:\n\nExemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..."
    });

    // 🔒 Aplica o esquema de segurança globalmente
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
#endregion


var app = builder.Build();

app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
