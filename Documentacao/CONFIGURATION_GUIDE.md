// Adicione esta seção ao seu appsettings.json para configurar o logging

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
  // ? NOVO: Configuração para erros e Unit of Work
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

## ?? Notas de Configuração

### Logging
- `Default: Information` - Logs normais
- `Microsoft: Warning` - Reduz ruído de logs do framework
- `EntityFrameworkCore: Information` - Mostra queries SQL
- `SNI_Events: Debug` - Logs detalhados da aplicação

### Unit of Work
- `TransactionTimeout: 30` - Timeout em segundos
- `AutoRollbackOnException: true` - Rollback automático
- `LogTransactions: true` - Log de transações

### Error Handling
- `ReturnDetailedErrors: false` - Não retorna stack trace ao cliente
- `LogStackTrace: true` - Log completo internamente
- `IncludeSensitiveData: false` - Não expõe dados sensíveis

---

## ?? Como usar as configurações

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
            _logger.LogInformation("Iniciando criação de usuário: {Email}", request.Email);
            
            var user = new User(...);
            await _userRepository.AddAsync(user);
            
            // Usar timeout da configuração
            var timeout = _configuration.GetValue<int>("UnitOfWork:TransactionTimeout");
            
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Usuário criado com sucesso: {UserId}", user.Id);
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Email}", request.Email);
            await _unitOfWork.RollbackAsync();
            return Result.Failure<User>($"Erro ao criar usuário: {ex.Message}");
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
        var message = returnDetailed ? exception.Message : "Um erro ocorreu ao processar sua solicitação";

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

## ? Ativação de Configurações

No seu `Program.cs`, já está tudo configurado:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ? Configurações carregadas automaticamente
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// ? Serviços registrados
builder.Services.AddLogging();
builder.Services.AddInfrastructure();

var app = builder.Build();

// ? Middleware registrado
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.Run();
```

---

## ?? Segurança (Importante!)

### Nunca exponha em Production:
- ? Stack trace completo
- ? Nomes de tabelas
- ? Queries SQL
- ? Dados sensíveis

### Sempre em Produção:
- ? `ReturnDetailedErrors: false`
- ? `IncludeSensitiveData: false`
- ? Logging mínimo (Warning+)

---

## ?? Exemplo de Log

```
[11:23:45.123 Information] Iniciando criação de usuário: joao@email.com
[11:23:45.234 Information] Validando CPF: 123.456.789-00
[11:23:45.345 Information] Verificando email duplicado
[11:23:45.456 Information] Criando entidade User
[11:23:45.567 Information] Persistindo no repositório
[11:23:45.678 Information] Confirmando transação (CommitAsync)
[11:23:45.789 Information] Usuário criado com sucesso: 42

? Erro:
[11:23:45.890 Error] Erro ao criar usuário: joao@email.com
System.InvalidOperationException: Email já existe
at SNI_Events.Application.UseCases.User.CreateUserUseCase.ExecuteAsync() in CreateUserUseCase.cs:line 65
at SNI_Events.Application.Services.UserService.CreateAsync() in UserService.cs:line 52
```

---

**Configuração:** Opcional mas Recomendada  
**Impacto:** Alto em produção  
**Complexidade:** Baixa
