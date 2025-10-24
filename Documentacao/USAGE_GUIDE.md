# Guia de Uso das Melhorias Implementadas

## ?? Vis�o Geral

Este guia descreve como usar as melhorias implementadas na arquitetura do projeto SNI_Events para manter a conformidade com Clean Architecture e SOLID.

---

## 1?? Unit of Work Pattern

### O que foi mudado?

Antes, cada reposit�rio chamava `SaveChangesAsync()` individualmente. Agora, todas as altera��es s�o confirmadas centralizadamente.

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
            
            // Opera��es no reposit�rio N�O salvam imediatamente
            await _userRepository.AddAsync(user);
            
            // Confirma todas as altera��es de uma vez
            await _unitOfWork.CommitAsync();
            
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            // Reverte todas as altera��es em caso de erro
            await _unitOfWork.RollbackAsync();
            return Result.Failure<User>(ex.Message);
        }
    }
}
```

### Benef�cios

? Transa��es at�micas - tudo ou nada  
? Consist�ncia garantida  
? Rollback autom�tico em caso de erro  
? Uma �nica chamada para confirmar

---

## 2?? Result Pattern

### O que foi mudado?

Antes usavam exce��es para erros de neg�cio. Agora usam `Result<T>` que encapsula sucesso/erro.

### Como usar?

```csharp
// Retornar sucesso
return Result.Success(user);

// Retornar erro
return Result.Failure<User>("Email j� existe");

// Consumir o resultado
var result = await _useCase.ExecuteAsync(request, userId);

if (!result.IsSuccess)
{
    // result.Error cont�m a mensagem de erro
    return BadRequest(new { error = result.Error });
}

// result.Value cont�m o objeto
var user = result.Value;
```

### Benef�cios

? Sem overhead de exce��es  
? Melhor performance  
? Tratamento expl�cito de erros  
? Type-safe

---

## 3?? Specification Pattern

### O que foi mudado?

Queries complexas agora est�o centralizadas em Specification classes.

### Specifications Dispon�veis

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

### M�todos do Reposit�rio

```csharp
// Uma entidade
var user = await _userRepository.FindBySpecificationAsync(spec);

// M�ltiplas entidades
var users = await _userRepository.FindAllBySpecificationAsync(spec);
```

### Benef�cios

? Queries reutiliz�veis  
? L�gica de filtro centralizada  
? F�cil de testar  
? DDD Pattern

---

## 4?? Global Exception Handler Middleware

### O que foi mudado?

Todas as exce��es n�o capturadas agora s�o tratadas globalmente e retornam JSON padronizado.

### Tratamento Autom�tico

```
Tipo de Exce��o            ? Status HTTP
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
  "message": "Um erro ocorreu ao processar sua solicita��o",
  "errors": ["Descri��o do erro espec�fico"]
}
```

### Exemplo

```csharp
// Esta exce��o ser� automaticamente capturada e retornar� 400
throw new ArgumentException("Email inv�lido");

// Resposta HTTP 400:
{
  "success": false,
  "message": "Um erro ocorreu ao processar sua solicita��o",
  "errors": ["Email inv�lido"]
}
```

### Benef�cios

? Tratamento consistente de erros  
? Sem duplica��o de try-catch  
? Respostas padronizadas  
? Logging autom�tico

---

## 5?? Base Use Case

### O que foi mudado?

Criada classe base `BaseUseCase` com m�todos comuns reutiliz�veis.

### Como usar?

```csharp
using SNI_Events.Application.UseCases.Base;

public class CreateUserUseCase : BaseUseCase
{
    public async Task<Result<User>> ExecuteAsync(CreateUserRequest request)
    {
        // Usar m�todo base para tratamento padronizado
        return await ExecuteWithTransactionAsync(
            async () => 
            {
                // Sua l�gica aqui
                var user = new User(...);
                return user;
            },
            "Erro ao criar usu�rio"
        );
    }
}
```

### M�todos Dispon�veis

```csharp
// Tratamento de erro standardizado
protected static Result<T> HandleError<T>(Exception ex, string userMessage)

// Executar com tratamento de transa��o autom�tico
protected static async Task<Result<T>> ExecuteWithTransactionAsync<T>(
    Func<Task<T>> operation,
    string errorMessage)
```

### Benef�cios

? Reduz duplica��o  
? Padr�o consistente  
? Reutiliz�vel

---

## ?? Checklist para Novos Use Cases

Ao criar um novo Use Case, siga este checklist:

- [ ] Herda de `BaseUseCase` (opcional mas recomendado)
- [ ] Recebe `IUnitOfWork` no construtor
- [ ] Chama `await _unitOfWork.CommitAsync()` ap�s persistir
- [ ] Chama `await _unitOfWork.RollbackAsync()` no catch
- [ ] Retorna `Result<T>` em vez de lan�ar exce��es
- [ ] Usa Specifications para queries
- [ ] Tem classe Request separada ou dentro do Use Case
- [ ] Valida��es acontecem antes de opera��es de BD

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
                return Result.Failure<YourEntity>("Requisi��o inv�lida");

            try
            {
                // Sua l�gica aqui
                var entity = new YourEntity(...);
                
                await _repository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return Result.Success(entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return HandleError<YourEntity>(ex, "Erro ao processar opera��o");
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

## ?? Melhorias no Fluxo de Requisi��o

### Antes
```
Request ? Controller ? Service ? Repository ? SaveChanges ? Response
          ? Sem tratamento global de erro
          ? SaveChanges em v�rios lugares
          ? Exce��es n�o tratadas
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
? Transa��es at�micas
? C�digo limpo e test�vel
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

## ?? Pr�ximas Melhorias Sugeridas

- [ ] Implementar CQRS para separar leitura/escrita
- [ ] Adicionar Event Sourcing para auditoria
- [ ] Implementar Saga Pattern para transa��es distribu�das
- [ ] Cache com Redis
- [ ] Background Jobs com Hangfire
- [ ] Logging estruturado com Serilog

---

## ?? Suporte

Para d�vidas sobre as implementa��es, consulte:

1. `ARCHITECTURE_IMPROVEMENTS.md` - Vis�o geral das mudan�as
2. Coment�rios no c�digo dos arquivos modificados
3. Use Cases existentes como exemplos
4. Specifications existentes como refer�ncia

---

**�ltima atualiza��o:** 2025  
**Vers�o:** 1.0  
**Status:** ? Em Produ��o
