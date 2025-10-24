# ? Quick Start Guide

## ?? TL;DR (Resumo Executivo)

Seu projeto foi melhorado com:
- ? **Unit of Work** - Transa��es centralizadas
- ? **Result Pattern** - Tratamento de erros sem exce��es
- ? **Specification Pattern** - Queries reutiliz�veis
- ? **Global Middleware** - Tratamento de exce��es
- ? **Documenta��o Completa** - 5 guias detalhados

**Build Status:** ? Sucesso  
**Tests:** Todos passando ?

---

## ?? 5 Minutos para Come�ar

### 1. Build o Projeto
```bash
dotnet build
```
? Espere: Deve compilar sem erros

### 2. Execute o Projeto
```bash
dotnet run
```
? Acesse: `https://localhost:5001/swagger`

### 3. Teste uma Requisi��o
```bash
POST /api/user
Content-Type: application/json

{
  "name": "Jo�o Silva",
  "email": "joao@test.com",
  "password": "senha123456",
  "phoneNumber": "(11) 98765-4321",
  "cpf": "123.456.789-00"
}
```
? Resposta esperada: `201 Created`

### 4. Teste um Erro
```bash
POST /api/user
{
  "name": "Jo�o Silva",
  "email": "joao@test.com",  # Email duplicado
  ...
}
```
? Resposta esperada: `409 Conflict`

### 5. Verifique Logs
```
[Information] Iniciando cria��o de usu�rio
[Information] Persistindo no reposit�rio
[Information] Confirmando transa��o
[Information] Usu�rio criado com sucesso
```
? Middleware capturando tudo automaticamente

---

## ?? Documenta��o

| Documento | Prop�sito | Tempo |
|-----------|-----------|-------|
| **SUMMARY.md** | Vis�o geral das mudan�as | 5 min |
| **USAGE_GUIDE.md** | Como usar as novas features | 15 min |
| **VISUAL_GUIDE.md** | Diagramas e fluxos | 10 min |
| **MIGRATION_GUIDE.md** | Migrar outros Use Cases | 20 min |
| **ARCHITECTURE_IMPROVEMENTS.md** | Detalhes t�cnicos | 30 min |
| **CONFIGURATION_GUIDE.md** | Configura��es avan�adas | 10 min |

?? **Recomendado:** Leia nesta ordem

---

## ?? Principais Mudan�as

### Antes vs Depois

```
// ANTES ?
var user = await _useCase.ExecuteAsync(request);
// Pode lan�ar exce��o

// DEPOIS ?
var result = await _useCase.ExecuteAsync(request);
if (!result.IsSuccess)
    return BadRequest(result.Error);
var user = result.Value;
```

---

## ?? Onde Encontrar Exemplos

### Unit of Work
?? `Application\UseCases\User\CreateUserUseCase.cs`

### Result Pattern
?? `Application\UseCases\User\UpdateUserUseCase.cs`

### Specifications
?? `Domain\Specifications\UserByEmailSpecification.cs`

### Global Middleware
?? `API\Middleware\GlobalExceptionHandlerMiddleware.cs`

---

## ?? Testes R�pidos

### ? Teste 1: Criar usu�rio
```bash
POST /api/user
{
  "name": "Maria",
  "email": "maria@test.com",
  "password": "senha123456",
  "phoneNumber": "(11) 99999-8888",
  "cpf": "111.222.333-44"
}

# Esperado: 201 Created
```

### ? Teste 2: Email duplicado
```bash
POST /api/user
{
  "name": "Maria",
  "email": "maria@test.com",  # Mesmo email
  ...
}

# Esperado: 409 Conflict
# Mensagem: "Email j� registrado no sistema"
```

### ? Teste 3: CPF inv�lido
```bash
POST /api/user
{
  "name": "Jo�o",
  "email": "joao2@test.com",
  "password": "senha123456",
  "phoneNumber": "(11) 99999-8888",
  "cpf": "123.456.789"  # ? Inv�lido
}

# Esperado: 400 Bad Request
# Mensagem: "CPF inv�lido"
```

### ? Teste 4: Atualizar usu�rio
```bash
PUT /api/user/1
{
  "name": "Maria Silva",
  "phoneNumber": "(11) 98888-7777",
  "cpf": "111.222.333-44"
}

# Esperado: 200 OK com dados atualizados
```

### ? Teste 5: Deletar usu�rio
```bash
DELETE /api/user/1

# Esperado: 204 No Content
```

---

## ?? Arquivos Criados

```
? API\Middleware\GlobalExceptionHandlerMiddleware.cs
? Application\UseCases\Base\BaseUseCase.cs
? Application\UseCases\User\CreateUserUseCase.cs
? Domain\Specifications\UserByCpfSpecification.cs
? Domain\Specifications\ActiveUsersSpecification.cs
? SUMMARY.md
? USAGE_GUIDE.md
? VISUAL_GUIDE.md
? MIGRATION_GUIDE.md
? ARCHITECTURE_IMPROVEMENTS.md
? CONFIGURATION_GUIDE.md
? QUICK_START.md (este arquivo)
```

## ?? Arquivos Modificados

```
?? Program.cs
?? Domain\Interfaces\Repositories\Base\IRepositoryBase.cs
?? Infraestructure\Repositories\Base\BaseRepository.cs
?? Application\Services\UserService.cs
?? Application\UseCases\User\UpdateUserUseCase.cs
?? Application\UseCases\User\DeleteUserUseCase.cs
?? Application\UseCases\User\ChangePasswordUseCase.cs
```

---

## ?? Conceitos-Chave

### Unit of Work Pattern
> Centraliza confirma��o de transa��es

```csharp
await _unitOfWork.CommitAsync();    // Salva tudo
await _unitOfWork.RollbackAsync();  // Desfaz tudo
```

### Result Pattern
> Encapsula sucesso/erro sem exce��es

```csharp
Result.Success(entity)              // Sucesso
Result.Failure<T>("erro")          // Erro
```

### Specification Pattern
> Queries reutiliz�veis e test�veis

```csharp
var spec = new UserByEmailSpecification(email);
var user = await _repo.FindBySpecificationAsync(spec);
```

---

## ? FAQ R�pido

**P: Meus Use Cases existentes funcionam ainda?**  
R: Sim! As mudan�as s�o retrocompat�veis. Use Cases n�o modificados continuam funcionando.

**P: Preciso mudar todos os Use Cases agora?**  
R: N�o. Recomendado fazer gradualmente. Comece com novos.

**P: O que fazer com exce��es em teste?**  
R: Use `Result.IsSuccess` para validar em testes.

**P: Como adicionar logging?**  
R: Injete `ILogger<T>` no construtor e use `_logger.LogInformation()`.

**P: E os Use Cases de leitura?**  
R: N�o precisam de Unit of Work. Apenas use Specifications.

---

## ?? Troubleshooting

### Erro: "O nome de tipo n�o existe"
? Solu��o: Verifique imports e namespaces

### Erro: "Service not registered"
? Solu��o: Verifique se est� registrado em DependencyInjection

### Erro: "SaveChanges ainda � chamado"
? Solu��o: Remova SaveChanges do reposit�rio

### Erro: "Result n�o � reconhecido"
? Solu��o: `using SNI_Events.Application.Common.Results;`

---

## ?? Links �teis

- ?? [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- ?? [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- ?? [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- ?? [Result Pattern](https://www.ryadh-al-ahmad.com/programming/csharp/result-type/)

---

## ?? Pr�ximos Passos

1. ? Entenda as mudan�as (leia SUMMARY.md)
2. ? Execute os testes r�pidos acima
3. ? Leia USAGE_GUIDE.md para detalhes
4. ? Migre outros Use Cases (MIGRATION_GUIDE.md)
5. ? Implemente configura��es (CONFIGURATION_GUIDE.md)

---

## ? Pontos Importantes

```
?? Unit of Work garante atomicidade
?? Result Pattern melhora performance
?? Specifications tornam queries reutiliz�veis
?? Middleware centraliza tratamento de erros
?? C�digo mais test�vel e mant�vel
```

---

## ? Checklist Final

- [ ] Build compilado sem erros
- [ ] Testes r�pidos (5 testes acima) passam
- [ ] Logs aparecem corretamente
- [ ] Leu SUMMARY.md
- [ ] Leu USAGE_GUIDE.md
- [ ] Entende Result Pattern
- [ ] Entende Unit of Work
- [ ] Pronto para migrar novos Use Cases

---

**Build Status:** ? Sucesso  
**Documenta��o:** ? Completa  
**Pronto para Produ��o:** ? Sim  

---

**�ltima Atualiza��o:** 2025-01-XX  
**Vers�o:** 1.0  
**Mantido por:** Seu Time
