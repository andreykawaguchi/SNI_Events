# Guia de Uso das Melhorias Implementadas

## ?? Visão Geral

Este guia descreve como usar as melhorias implementadas na arquitetura do projeto SNI_Events para manter a conformidade com Clean Architecture e SOLID.

---

## 1?? Unit of Work Pattern

### O que foi mudado?

Antes, cada repositório chamava `SaveChangesAsync()` individualmente. Agora, todas as alterações são confirmadas centralizadamente.

### Como usar?

```csharp
// No seu Use Case
public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<User>> ExecuteAsync(CreateUserRequest request, long? userId)
    {
        try
        {
            var user = new User(...);
            
            // Operações no repositório NÃO salvam imediatamente
            await _userRepository.AddAsync(user);
            
            // Confirma todas as alterações de uma vez
            await _unitOfWork.CommitAsync();
            
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            // Reverte todas as alterações em caso de erro
            await _unitOfWork.RollbackAsync();
            return Result.Failure<User>(ex.Message);
        }
    }
}
```

### Benefícios

? Transações atômicas - tudo ou nada  
? Consistência garantida  
? Rollback automático em caso de erro  
? Uma única chamada para confirmar

---

## 2?? Result Pattern

### O que foi mudado?

Antes usavam exceções para erros de negócio. Agora usam `Result<T>` que encapsula sucesso/erro.

### Como usar?

```csharp
// Retornar sucesso
return Result.Success(user);

// Retornar erro
return Result.Failure<User>("Email já existe");

// Consumir o resultado
var result = await _useCase.ExecuteAsync(request, userId);

if (!result.IsSuccess)
{
    // result.Error contém a mensagem de erro
    return BadRequest(new { error = result.Error });
}

// result.Value contém o objeto
var user = result.Value;
```

### Benefícios

? Sem overhead de exceções  
? Melhor performance  
? Tratamento explícito de erros  
? Type-safe

---

## 3?? Specification Pattern

### O que foi mudado?

Queries complexas agora estão centralizadas em Specification classes.

### Specifications Disponíveis

1. **UserByEmailSpecification**
```csharp
var spec = new UserByEmailSpecification("user@email.com");
var user = await _userRepository.FindBySpecificationAsync(spec);
```

2. **UserByCpfSpecification**
```csharp
var spec = new UserByCpfSpecification("123.456.789-00");
var user = await _userRepository.FindBySpecificationAsync(spec);
```

3. **ActiveUsersSpecification**
```csharp
var spec = new ActiveUsersSpecification();
var activeUsers = await _userRepository.FindAllBySpecificationAsync(spec);
```

### Como criar uma nova Specification

```csharp
using SNI_Events.Domain.Interfaces.Specifications;
using SNI_Events.Domain.Entities;

public class UserByNameSpecification : ISpecification<User>
{
    private readonly string _name;

    public UserByNameSpecification(string name)
    {
        _name = name;
    }

    public IQueryable<User> ToQuery(IQueryable<User> query)
    {
        if (string.IsNullOrWhiteSpace(_name))
            return query;

        return query.Where(u => u.Name.Contains(_name));
    }
}
```

### Métodos do Repositório

```csharp
// Uma entidade
var user = await _userRepository.FindBySpecificationAsync(spec);

// Múltiplas entidades
var users = await _userRepository.FindAllBySpecificationAsync(spec);
```

### Benefícios

? Queries reutilizáveis  
? Lógica de filtro centralizada  
? Fácil de testar  
? DDD Pattern

---

## 4?? Global Exception Handler Middleware

### O que foi mudado?

Todas as exceções não capturadas agora são tratadas globalmente e retornam JSON padronizado.

### Tratamento Automático

```
Tipo de Exceção            ? Status HTTP
?????????????????????????????????????????
ArgumentNullException      ? 400 Bad Request
ArgumentException          ? 400 Bad Request
InvalidOperationException  ? 409 Conflict
KeyNotFoundException       ? 404 Not Found
Qualquer outro erro        ? 500 Internal Server
```

### Resposta de Erro

```json
{
  "success": false,
  "message": "Um erro ocorreu ao processar sua solicitação",
  "errors": ["Descrição do erro específico"]
}
```

### Exemplo

```csharp
// Esta exceção será automaticamente capturada e retornará 400
throw new ArgumentException("Email inválido");

// Resposta HTTP 400:
{
  "success": false,
  "message": "Um erro ocorreu ao processar sua solicitação",
  "errors": ["Email inválido"]
}
```

### Benefícios

? Tratamento consistente de erros  
? Sem duplicação de try-catch  
? Respostas padronizadas  
? Logging automático

---

## 5?? Base Use Case

### O que foi mudado?

Criada classe base `BaseUseCase` com métodos comuns reutilizáveis.

### Como usar?

```csharp
using SNI_Events.Application.UseCases.Base;

public class CreateUserUseCase : BaseUseCase
{
    public async Task<Result<User>> ExecuteAsync(CreateUserRequest request)
    {
        // Usar método base para tratamento padronizado
        return await ExecuteWithTransactionAsync(
            async () => 
            {
                // Sua lógica aqui
                var user = new User(...);
                return user;
            },
            "Erro ao criar usuário"
        );
    }
}
```

### Métodos Disponíveis

```csharp
// Tratamento de erro standardizado
protected static Result<T> HandleError<T>(Exception ex, string userMessage)

// Executar com tratamento de transação automático
protected static async Task<Result<T>> ExecuteWithTransactionAsync<T>(
    Func<Task<T>> operation,
    string errorMessage)
```

### Benefícios

? Reduz duplicação  
? Padrão consistente  
? Reutilizável

---

## ?? Checklist para Novos Use Cases

Ao criar um novo Use Case, siga este checklist:

- [ ] Herda de `BaseUseCase` (opcional mas recomendado)
- [ ] Recebe `IUnitOfWork` no construtor
- [ ] Chama `await _unitOfWork.CommitAsync()` após persistir
- [ ] Chama `await _unitOfWork.RollbackAsync()` no catch
- [ ] Retorna `Result<T>` em vez de lançar exceções
- [ ] Usa Specifications para queries
- [ ] Tem classe Request separada ou dentro do Use Case
- [ ] Validações acontecem antes de operações de BD

### Template

```csharp
using SNI_Events.Application.Common.Results;
using SNI_Events.Application.UseCases.Base;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.Application.UseCases.YourDomain
{
    public class YourNewUseCase : BaseUseCase
    {
        private readonly IYourRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public YourNewUseCase(IYourRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<YourEntity>> ExecuteAsync(YourRequest request, long? userId)
        {
            if (request == null)
                return Result.Failure<YourEntity>("Requisição inválida");

            try
            {
                // Sua lógica aqui
                var entity = new YourEntity(...);
                
                await _repository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return Result.Success(entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return HandleError<YourEntity>(ex, "Erro ao processar operação");
            }
        }
    }

    public class YourRequest
    {
        public string Property { get; set; }
    }
}
```

---

## ?? Melhorias no Fluxo de Requisição

### Antes
```
Request ? Controller ? Service ? Repository ? SaveChanges ? Response
          ? Sem tratamento global de erro
          ? SaveChanges em vários lugares
          ? Exceções não tratadas
```

### Depois
```
Request ? GlobalExceptionHandlerMiddleware
          ?
          Controller ? Service ? Use Case
          ?
          Repository ? Specification
          ?
          Result<T> Pattern
          ?
          Unit of Work ? CommitAsync/RollbackAsync
          ?
          Response JSON (Padronizada)
          
? Tratamento de erro centralizado
? Transações atômicas
? Código limpo e testável
```

---

## ?? Testabilidade

### Teste de Use Case

```csharp
[TestClass]
public class CreateUserUseCaseTests
{
    private Mock<IUserRepository> _repositoryMock;
    private Mock<IUserDomainService> _serviceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateUserUseCase _useCase;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _serviceMock = new Mock<IUserDomainService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _useCase = new CreateUserUseCase(
            _repositoryMock.Object,
            _serviceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [TestMethod]
    public async Task ExecuteAsync_WithValidEmail_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "test@test.com", ... };
        _serviceMock.Setup(s => s.IsValidEmail(It.IsAny<string>())).Returns(true);
        _serviceMock.Setup(s => s.IsValidCpf(It.IsAny<string>())).Returns(true);
        _serviceMock.Setup(s => s.EmailAlreadyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _serviceMock.Setup(s => s.CpfAlreadyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        var result = await _useCase.ExecuteAsync(request, 1);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
}
```

---

## ?? Próximas Melhorias Sugeridas

- [ ] Implementar CQRS para separar leitura/escrita
- [ ] Adicionar Event Sourcing para auditoria
- [ ] Implementar Saga Pattern para transações distribuídas
- [ ] Cache com Redis
- [ ] Background Jobs com Hangfire
- [ ] Logging estruturado com Serilog

---

## ?? Suporte

Para dúvidas sobre as implementações, consulte:

1. `ARCHITECTURE_IMPROVEMENTS.md` - Visão geral das mudanças
2. Comentários no código dos arquivos modificados
3. Use Cases existentes como exemplos
4. Specifications existentes como referência

---

**Última atualização:** 2025  
**Versão:** 1.0  
**Status:** ? Em Produção
