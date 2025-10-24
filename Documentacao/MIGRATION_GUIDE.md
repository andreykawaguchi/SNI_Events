# Guia de Migração de Use Cases Existentes

## ?? Resumo

Este documento descreve como migrar Use Cases existentes para usar as novas melhorias (Unit of Work, Result Pattern, Specifications).

---

## ?? Categorias de Use Cases

### 1. Use Cases de Escrita (CREATE, UPDATE, DELETE)
**Requerem:** Unit of Work + Result Pattern

- ? `CreateUserUseCase` - Já migrado
- ? `UpdateUserUseCase` - Já migrado
- ? `DeleteUserUseCase` - Já migrado
- ? `ChangePasswordUseCase` - Já migrado
- ?? `AddUserToDinnerUseCase` - Precisa migração
- ?? `UpdateUserDinnerPaymentStatusUseCase` - Precisa migração
- ?? `UpdateUserDinnerPresenceUseCase` - Precisa migração

### 2. Use Cases de Leitura (GET, SEARCH)
**Requerem:** Specifications apenas (Unit of Work é opcional)

- ? `GetUserByIdUseCase` - Não precisa migração
- ? `GetPagedUsersUseCase` - Não precisa migração

---

## ?? Padrão de Migração

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
            ?? throw new KeyNotFoundException("Usuário não encontrado");

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
            return Result.Failure<User>("Requisição inválida");

        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result.Failure<User>($"Usuário com ID {id} não encontrado");  // ? Result em vez de throw

            user.Update(request.Name, request.PhoneNumber, request.Role);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();  // ? Commit centralizado

            return Result.Success(user);  // ? Success
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();  // ? Rollback automático
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
        return Result.Failure<UserDinner>("ID de usuário inválido");
    
    if (dinnerId <= 0)
        return Result.Failure<UserDinner>("ID de jantar inválido");
    
    // ... resto do código
}
```

### Passo 4: Remover Throws de Negócio

```csharp
// Antes
var user = await _userRepository.GetByIdAsync(userId)
    ?? throw new KeyNotFoundException("Usuário não encontrado");

// Depois
var user = await _userRepository.GetByIdAsync(userId);
if (user == null)
    return Result.Failure<UserDinner>("Usuário não encontrado");
```

### Passo 5: Adicionar Try-Catch com Commit/Rollback

```csharp
try
{
    // Suas operações
    await _repository.AddAsync(entity);
    await _repository.UpdateAsync(entity);
    
    // Confirmar uma única vez
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

## ?? Use Cases Prontos para Migração

### AddUserToDinnerUseCase

**Status:** ?? Precisa migração  
**Tipo:** Escrita

```csharp
// Adicionar ao construtor
private readonly IUnitOfWork _unitOfWork;

// Mudar assinatura
public async Task<Result<UserDinner>> ExecuteAsync(...)

// Adicionar validações e Result Pattern
if (user == null)
    return Result.Failure<UserDinner>("Usuário não encontrado");

// Adicionar try-catch com Unit of Work
try
{
    // operações...
    await _unitOfWork.CommitAsync();
    return Result.Success(userDinner);
}
catch (Exception ex)
{
    await _unitOfWork.RollbackAsync();
    return Result.Failure<UserDinner>($"Erro ao adicionar usuário ao jantar: {ex.Message}");
}
```

### UpdateUserDinnerPaymentStatusUseCase

**Status:** ?? Precisa migração  
**Tipo:** Escrita

Mesmo padrão do `UpdateUserUseCase`

### UpdateUserDinnerPresenceUseCase

**Status:** ?? Precisa migração  
**Tipo:** Escrita

Mesmo padrão do `UpdateUserUseCase`

---

## ? Checklist de Migração

Para cada Use Case de escrita:

- [ ] Adicionar `IUnitOfWork` no construtor
- [ ] Mudar tipo de retorno para `Result<T>`
- [ ] Adicionar validação de entrada com `Result.Failure`
- [ ] Remover `throw new KeyNotFoundException`
- [ ] Remover `throw new ArgumentException`
- [ ] Adicionar try-catch com `CommitAsync()` e `RollbackAsync()`
- [ ] Atualizar consumidor (Service) para tratar `Result`
- [ ] Testar compilação
- [ ] Testar funcionamento

---

## ?? Exemplo Completo: Migração de AddUserToDinnerUseCase

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
            ?? throw new KeyNotFoundException($"Usuário {userId} não encontrado");

        var dinner = await _dinnerRepository.GetByIdAsync(dinnerId)
            ?? throw new KeyNotFoundException($"Jantar {dinnerId} não encontrado");

        if (user.UserDinners.Any(ud => ud.DinnerId == dinnerId))
            throw new InvalidOperationException("Usuário já associado a este jantar");

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
            return Result.Failure<UserDinner>("ID de usuário inválido");
        if (dinnerId <= 0)
            return Result.Failure<UserDinner>("ID de jantar inválido");

        try
        {
            // Buscar usuário
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserDinner>($"Usuário {userId} não encontrado");

            // Buscar jantar
            var dinner = await _dinnerRepository.GetByIdAsync(dinnerId);
            if (dinner == null)
                return Result.Failure<UserDinner>($"Jantar {dinnerId} não encontrado");

            // Verificar duplicata
            if (user.UserDinners.Any(ud => ud.DinnerId == dinnerId))
                return Result.Failure<UserDinner>("Usuário já associado a este jantar");

            // Criar associação
            var userDinner = new UserDinner(user, dinner, false, EStatusPayment.Pending, createdByUserId);
            
            // Persistir
            await _userRepository.UpdateAsync(user);
            
            // Confirmar transação
            await _unitOfWork.CommitAsync();  // ? Novo
            
            return Result.Success(userDinner);  // ? Novo
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();  // ? Novo
            return Result.Failure<UserDinner>($"Erro ao adicionar usuário ao jantar: {ex.Message}");  // ? Novo
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
    ? (se falhar, resultado é tratado como exceção)
    ?
UserController.Create
    ? (retorna erro ou sucesso)
    ?
Response JSON
```

Com as mudanças:

```
CreateUserUseCase (Result<User>)
    ?
UserService.CreateAsync (trata Result)
    ?
UserController.Create (retorna ApiResponse)
    ?
GlobalExceptionHandlerMiddleware (captura exceções não tratadas)
    ?
Response JSON (padronizado)
```

---

## ?? Referências

- Ver `CreateUserUseCase.cs` como exemplo de Use Case migrado
- Ver `UpdateUserUseCase.cs` como exemplo de Update migrado
- Ver `DeleteUserUseCase.cs` como exemplo de Delete migrado
- Ver `USAGE_GUIDE.md` para mais detalhes

---

**Status:** ? Pronto para implementação  
**Prioridade:** Alta  
**Esforço:** Médio (1-2 horas por Use Case)
