# Guia de Migra��o de Use Cases Existentes

## ?? Resumo

Este documento descreve como migrar Use Cases existentes para usar as novas melhorias (Unit of Work, Result Pattern, Specifications).

---

## ?? Categorias de Use Cases

### 1. Use Cases de Escrita (CREATE, UPDATE, DELETE)
**Requerem:** Unit of Work + Result Pattern

- ? `CreateUserUseCase` - J� migrado
- ? `UpdateUserUseCase` - J� migrado
- ? `DeleteUserUseCase` - J� migrado
- ? `ChangePasswordUseCase` - J� migrado
- ?? `AddUserToDinnerUseCase` - Precisa migra��o
- ?? `UpdateUserDinnerPaymentStatusUseCase` - Precisa migra��o
- ?? `UpdateUserDinnerPresenceUseCase` - Precisa migra��o

### 2. Use Cases de Leitura (GET, SEARCH)
**Requerem:** Specifications apenas (Unit of Work � opcional)

- ? `GetUserByIdUseCase` - N�o precisa migra��o
- ? `GetPagedUsersUseCase` - N�o precisa migra��o

---

## ?? Padr�o de Migra��o

### Antes (Sem Unit of Work)

```csharp
public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> ExecuteAsync(long id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Usu�rio n�o encontrado");

        user.Update(request.Name, request.PhoneNumber, request.Role);
        await _userRepository.UpdateAsync(user);  // ? SaveChanges aqui

        return user;
    }
}
```

### Depois (Com Unit of Work + Result)

```csharp
public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;  // ? Novo

    public UpdateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;  // ? Novo
    }

    public async Task<Result<User>> ExecuteAsync(long id, UpdateUserRequest request)  // ? Result<T>
    {
        if (request == null)
            return Result.Failure<User>("Requisi��o inv�lida");

        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result.Failure<User>($"Usu�rio com ID {id} n�o encontrado");  // ? Result em vez de throw

            user.Update(request.Name, request.PhoneNumber, request.Role);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();  // ? Commit centralizado

            return Result.Success(user);  // ? Success
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();  // ? Rollback autom�tico
            return Result.Failure<User>($"Erro ao atualizar: {ex.Message}");
        }
    }
}
```

---

## ?? Passo a Passo

### Passo 1: Adicionar IUnitOfWork no Construtor

```csharp
// Antes
public AddUserToDinnerUseCase(IUserRepository userRepository)

// Depois
public AddUserToDinnerUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    _userRepository = userRepository;
    _unitOfWork = unitOfWork;
}
```

### Passo 2: Mudar Tipo de Retorno para Result<T>

```csharp
// Antes
public async Task<UserDinner> ExecuteAsync(...)

// Depois
public async Task<Result<UserDinner>> ExecuteAsync(...)
```

### Passo 3: Validar Entrada

```csharp
public async Task<Result<UserDinner>> ExecuteAsync(long userId, long dinnerId)
{
    if (userId <= 0)
        return Result.Failure<UserDinner>("ID de usu�rio inv�lido");
    
    if (dinnerId <= 0)
        return Result.Failure<UserDinner>("ID de jantar inv�lido");
    
    // ... resto do c�digo
}
```

### Passo 4: Remover Throws de Neg�cio

```csharp
// Antes
var user = await _userRepository.GetByIdAsync(userId)
    ?? throw new KeyNotFoundException("Usu�rio n�o encontrado");

// Depois
var user = await _userRepository.GetByIdAsync(userId);
if (user == null)
    return Result.Failure<UserDinner>("Usu�rio n�o encontrado");
```

### Passo 5: Adicionar Try-Catch com Commit/Rollback

```csharp
try
{
    // Suas opera��es
    await _repository.AddAsync(entity);
    await _repository.UpdateAsync(entity);
    
    // Confirmar uma �nica vez
    await _unitOfWork.CommitAsync();
    
    return Result.Success(entity);
}
catch (Exception ex)
{
    await _unitOfWork.RollbackAsync();
    return Result.Failure<UserDinner>($"Erro: {ex.Message}");
}
```

### Passo 6: Atualizar Consumidores

```csharp
// Antes (no Service)
await _useCase.ExecuteAsync(userId, dinnerId);

// Depois
var result = await _useCase.ExecuteAsync(userId, dinnerId);
if (!result.IsSuccess)
    throw new InvalidOperationException(result.Error);

var userDinner = result.Value;
```

---

## ?? Use Cases Prontos para Migra��o

### AddUserToDinnerUseCase

**Status:** ?? Precisa migra��o  
**Tipo:** Escrita

```csharp
// Adicionar ao construtor
private readonly IUnitOfWork _unitOfWork;

// Mudar assinatura
public async Task<Result<UserDinner>> ExecuteAsync(...)

// Adicionar valida��es e Result Pattern
if (user == null)
    return Result.Failure<UserDinner>("Usu�rio n�o encontrado");

// Adicionar try-catch com Unit of Work
try
{
    // opera��es...
    await _unitOfWork.CommitAsync();
    return Result.Success(userDinner);
}
catch (Exception ex)
{
    await _unitOfWork.RollbackAsync();
    return Result.Failure<UserDinner>($"Erro ao adicionar usu�rio ao jantar: {ex.Message}");
}
```

### UpdateUserDinnerPaymentStatusUseCase

**Status:** ?? Precisa migra��o  
**Tipo:** Escrita

Mesmo padr�o do `UpdateUserUseCase`

### UpdateUserDinnerPresenceUseCase

**Status:** ?? Precisa migra��o  
**Tipo:** Escrita

Mesmo padr�o do `UpdateUserUseCase`

---

## ? Checklist de Migra��o

Para cada Use Case de escrita:

- [ ] Adicionar `IUnitOfWork` no construtor
- [ ] Mudar tipo de retorno para `Result<T>`
- [ ] Adicionar valida��o de entrada com `Result.Failure`
- [ ] Remover `throw new KeyNotFoundException`
- [ ] Remover `throw new ArgumentException`
- [ ] Adicionar try-catch com `CommitAsync()` e `RollbackAsync()`
- [ ] Atualizar consumidor (Service) para tratar `Result`
- [ ] Testar compila��o
- [ ] Testar funcionamento

---

## ?? Exemplo Completo: Migra��o de AddUserToDinnerUseCase

### Antes

```csharp
public class AddUserToDinnerUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IDinnerRepository _dinnerRepository;
    private readonly IUserDinnerDomainService _userDinnerService;

    public AddUserToDinnerUseCase(
        IUserRepository userRepository,
        IDinnerRepository dinnerRepository,
        IUserDinnerDomainService userDinnerService)
    {
        _userRepository = userRepository;
        _dinnerRepository = dinnerRepository;
        _userDinnerService = userDinnerService;
    }

    public async Task<UserDinner> ExecuteAsync(long userId, long dinnerId, long? createdByUserId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"Usu�rio {userId} n�o encontrado");

        var dinner = await _dinnerRepository.GetByIdAsync(dinnerId)
            ?? throw new KeyNotFoundException($"Jantar {dinnerId} n�o encontrado");

        if (user.UserDinners.Any(ud => ud.DinnerId == dinnerId))
            throw new InvalidOperationException("Usu�rio j� associado a este jantar");

        var userDinner = new UserDinner(user, dinner, false, EStatusPayment.Pending, createdByUserId);
        
        await _userRepository.UpdateAsync(user);
        return userDinner;
    }
}
```

### Depois

```csharp
public class AddUserToDinnerUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IDinnerRepository _dinnerRepository;
    private readonly IUserDinnerDomainService _userDinnerService;
    private readonly IUnitOfWork _unitOfWork;  // ? Novo

    public AddUserToDinnerUseCase(
        IUserRepository userRepository,
        IDinnerRepository dinnerRepository,
        IUserDinnerDomainService userDinnerService,
        IUnitOfWork unitOfWork)  // ? Novo
    {
        _userRepository = userRepository;
        _dinnerRepository = dinnerRepository;
        _userDinnerService = userDinnerService;
        _unitOfWork = unitOfWork;  // ? Novo
    }

    public async Task<Result<UserDinner>> ExecuteAsync(long userId, long dinnerId, long? createdByUserId)  // ? Result
    {
        // Validar entrada
        if (userId <= 0)
            return Result.Failure<UserDinner>("ID de usu�rio inv�lido");
        if (dinnerId <= 0)
            return Result.Failure<UserDinner>("ID de jantar inv�lido");

        try
        {
            // Buscar usu�rio
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserDinner>($"Usu�rio {userId} n�o encontrado");

            // Buscar jantar
            var dinner = await _dinnerRepository.GetByIdAsync(dinnerId);
            if (dinner == null)
                return Result.Failure<UserDinner>($"Jantar {dinnerId} n�o encontrado");

            // Verificar duplicata
            if (user.UserDinners.Any(ud => ud.DinnerId == dinnerId))
                return Result.Failure<UserDinner>("Usu�rio j� associado a este jantar");

            // Criar associa��o
            var userDinner = new UserDinner(user, dinner, false, EStatusPayment.Pending, createdByUserId);
            
            // Persistir
            await _userRepository.UpdateAsync(user);
            
            // Confirmar transa��o
            await _unitOfWork.CommitAsync();  // ? Novo
            
            return Result.Success(userDinner);  // ? Novo
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();  // ? Novo
            return Result.Failure<UserDinner>($"Erro ao adicionar usu�rio ao jantar: {ex.Message}");  // ? Novo
        }
    }
}
```

---

## ?? Relacionamentos entre Use Cases

```
CreateUserUseCase
    ? (usa)
    ?
UserService.CreateAsync
    ? (se falhar, resultado � tratado como exce��o)
    ?
UserController.Create
    ? (retorna erro ou sucesso)
    ?
Response JSON
```

Com as mudan�as:

```
CreateUserUseCase (Result<User>)
    ?
UserService.CreateAsync (trata Result)
    ?
UserController.Create (retorna ApiResponse)
    ?
GlobalExceptionHandlerMiddleware (captura exce��es n�o tratadas)
    ?
Response JSON (padronizado)
```

---

## ?? Refer�ncias

- Ver `CreateUserUseCase.cs` como exemplo de Use Case migrado
- Ver `UpdateUserUseCase.cs` como exemplo de Update migrado
- Ver `DeleteUserUseCase.cs` como exemplo de Delete migrado
- Ver `USAGE_GUIDE.md` para mais detalhes

---

**Status:** ? Pronto para implementa��o  
**Prioridade:** Alta  
**Esfor�o:** M�dio (1-2 horas por Use Case)
