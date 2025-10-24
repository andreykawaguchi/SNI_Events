// Adicione esta se��o ao seu appsettings.json para configurar o logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Information",
      "SNI_Events": "Debug"
    },
    "Console": {
      "IncludeScopes": true
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "ExpirationMinutes": 60,
    "Issuer": "snievents",
    "Audience": "snieventsapi"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SNIEvents;Integrated Security=true;"
  },
  // ? NOVO: Configura��o para erros e Unit of Work
  "UnitOfWork": {
    "TransactionTimeout": 30,
    "AutoRollbackOnException": true,
    "LogTransactions": true
  },
  "ErrorHandling": {
    "ReturnDetailedErrors": false,
    "LogStackTrace": true,
    "IncludeSensitiveData": false
  }
}
```

---

## ?? Notas de Configura��o

### Logging
- `Default: Information` - Logs normais
- `Microsoft: Warning` - Reduz ru�do de logs do framework
- `EntityFrameworkCore: Information` - Mostra queries SQL
- `SNI_Events: Debug` - Logs detalhados da aplica��o

### Unit of Work
- `TransactionTimeout: 30` - Timeout em segundos
- `AutoRollbackOnException: true` - Rollback autom�tico
- `LogTransactions: true` - Log de transa��es

### Error Handling
- `ReturnDetailedErrors: false` - N�o retorna stack trace ao cliente
- `LogStackTrace: true` - Log completo internamente
- `IncludeSensitiveData: false` - N�o exp�e dados sens�veis

---

## ?? Como usar as configura��es

### No seu Use Case

```csharp
public class CreateUserUseCase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserUseCase> _logger;

    public CreateUserUseCase(
        IConfiguration configuration,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateUserUseCase> logger)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<User>> ExecuteAsync(CreateUserRequest request, long? userId)
    {
        try
        {
            _logger.LogInformation("Iniciando cria��o de usu�rio: {Email}", request.Email);
            
            var user = new User(...);
            await _userRepository.AddAsync(user);
            
            // Usar timeout da configura��o
            var timeout = _configuration.GetValue<int>("UnitOfWork:TransactionTimeout");
            
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Usu�rio criado com sucesso: {UserId}", user.Id);
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usu�rio: {Email}", request.Email);
            await _unitOfWork.RollbackAsync();
            return Result.Failure<User>($"Erro ao criar usu�rio: {ex.Message}");
        }
    }
}
```

### No seu Middleware

```csharp
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var logStackTrace = _configuration.GetValue<bool>("ErrorHandling:LogStackTrace");
            
            if (logStackTrace)
                _logger.LogError(ex, "Unhandled exception occurred");
            else
                _logger.LogError("Unhandled exception occurred: {Message}", ex.Message);
            
            await HandleExceptionAsync(context, ex, _configuration);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IConfiguration configuration)
    {
        context.Response.ContentType = "application/json";

        var returnDetailed = configuration.GetValue<bool>("ErrorHandling:ReturnDetailedErrors");
        var message = returnDetailed ? exception.Message : "Um erro ocorreu ao processar sua solicita��o";

        var response = ApiResponse.Error(message, exception.Message);

        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.Conflict,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
```

---

## ?? Ambientes (Development, Staging, Production)

### appsettings.Development.json
```json
{
  "ErrorHandling": {
    "ReturnDetailedErrors": true,
    "LogStackTrace": true,
    "IncludeSensitiveData": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### appsettings.Staging.json
```json
{
  "ErrorHandling": {
    "ReturnDetailedErrors": false,
    "LogStackTrace": true,
    "IncludeSensitiveData": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Production.json
```json
{
  "ErrorHandling": {
    "ReturnDetailedErrors": false,
    "LogStackTrace": false,
    "IncludeSensitiveData": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

---

## ? Ativa��o de Configura��es

No seu `Program.cs`, j� est� tudo configurado:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ? Configura��es carregadas automaticamente
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// ? Servi�os registrados
builder.Services.AddLogging();
builder.Services.AddInfrastructure();

var app = builder.Build();

// ? Middleware registrado
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.Run();
```

---

## ?? Seguran�a (Importante!)

### Nunca exponha em Production:
- ? Stack trace completo
- ? Nomes de tabelas
- ? Queries SQL
- ? Dados sens�veis

### Sempre em Produ��o:
- ? `ReturnDetailedErrors: false`
- ? `IncludeSensitiveData: false`
- ? Logging m�nimo (Warning+)

---

## ?? Exemplo de Log

```
[11:23:45.123 Information] Iniciando cria��o de usu�rio: joao@email.com
[11:23:45.234 Information] Validando CPF: 123.456.789-00
[11:23:45.345 Information] Verificando email duplicado
[11:23:45.456 Information] Criando entidade User
[11:23:45.567 Information] Persistindo no reposit�rio
[11:23:45.678 Information] Confirmando transa��o (CommitAsync)
[11:23:45.789 Information] Usu�rio criado com sucesso: 42

? Erro:
[11:23:45.890 Error] Erro ao criar usu�rio: joao@email.com
System.InvalidOperationException: Email j� existe
at SNI_Events.Application.UseCases.User.CreateUserUseCase.ExecuteAsync() in CreateUserUseCase.cs:line 65
at SNI_Events.Application.Services.UserService.CreateAsync() in UserService.cs:line 52
```

---

**Configura��o:** Opcional mas Recomendada  
**Impacto:** Alto em produ��o  
**Complexidade:** Baixa
