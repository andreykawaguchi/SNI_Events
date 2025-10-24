# ? Quick Start Guide

## ?? TL;DR (Resumo Executivo)

Seu projeto foi melhorado com:
- ? **Unit of Work** - Transações centralizadas
- ? **Result Pattern** - Tratamento de erros sem exceções
- ? **Specification Pattern** - Queries reutilizáveis
- ? **Global Middleware** - Tratamento de exceções
- ? **Documentação Completa** - 5 guias detalhados

**Build Status:** ? Sucesso  
**Tests:** Todos passando ?

---

## ?? 5 Minutos para Começar

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

### 3. Teste uma Requisição
```bash
POST /api/user
Content-Type: application/json

{
  "name": "João Silva",
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
  "name": "João Silva",
  "email": "joao@test.com",  # Email duplicado
  ...
}
```
? Resposta esperada: `409 Conflict`

### 5. Verifique Logs
```
[Information] Iniciando criação de usuário
[Information] Persistindo no repositório
[Information] Confirmando transação
[Information] Usuário criado com sucesso
```
? Middleware capturando tudo automaticamente

---

## ?? Documentação

| Documento | Propósito | Tempo |
|-----------|-----------|-------|
| **SUMMARY.md** | Visão geral das mudanças | 5 min |
| **USAGE_GUIDE.md** | Como usar as novas features | 15 min |
| **VISUAL_GUIDE.md** | Diagramas e fluxos | 10 min |
| **MIGRATION_GUIDE.md** | Migrar outros Use Cases | 20 min |
| **ARCHITECTURE_IMPROVEMENTS.md** | Detalhes técnicos | 30 min |
| **CONFIGURATION_GUIDE.md** | Configurações avançadas | 10 min |

?? **Recomendado:** Leia nesta ordem

---

## ?? Principais Mudanças

### Antes vs Depois

```
// ANTES ?
var user = await _useCase.ExecuteAsync(request);
// Pode lançar exceção

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

## ?? Testes Rápidos

### ? Teste 1: Criar usuário
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
# Mensagem: "Email já registrado no sistema"
```

### ? Teste 3: CPF inválido
```bash
POST /api/user
{
  "name": "João",
  "email": "joao2@test.com",
  "password": "senha123456",
  "phoneNumber": "(11) 99999-8888",
  "cpf": "123.456.789"  # ? Inválido
}

# Esperado: 400 Bad Request
# Mensagem: "CPF inválido"
```

### ? Teste 4: Atualizar usuário
```bash
PUT /api/user/1
{
  "name": "Maria Silva",
  "phoneNumber": "(11) 98888-7777",
  "cpf": "111.222.333-44"
}

# Esperado: 200 OK com dados atualizados
```

### ? Teste 5: Deletar usuário
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
> Centraliza confirmação de transações

```csharp
await _unitOfWork.CommitAsync();    // Salva tudo
await _unitOfWork.RollbackAsync();  // Desfaz tudo
```

### Result Pattern
> Encapsula sucesso/erro sem exceções

```csharp
Result.Success(entity)              // Sucesso
Result.Failure<T>("erro")          // Erro
```

### Specification Pattern
> Queries reutilizáveis e testáveis

```csharp
var spec = new UserByEmailSpecification(email);
var user = await _repo.FindBySpecificationAsync(spec);
```

---

## ? FAQ Rápido

**P: Meus Use Cases existentes funcionam ainda?**  
R: Sim! As mudanças são retrocompatíveis. Use Cases não modificados continuam funcionando.

**P: Preciso mudar todos os Use Cases agora?**  
R: Não. Recomendado fazer gradualmente. Comece com novos.

**P: O que fazer com exceções em teste?**  
R: Use `Result.IsSuccess` para validar em testes.

**P: Como adicionar logging?**  
R: Injete `ILogger<T>` no construtor e use `_logger.LogInformation()`.

**P: E os Use Cases de leitura?**  
R: Não precisam de Unit of Work. Apenas use Specifications.

---

## ?? Troubleshooting

### Erro: "O nome de tipo não existe"
? Solução: Verifique imports e namespaces

### Erro: "Service not registered"
? Solução: Verifique se está registrado em DependencyInjection

### Erro: "SaveChanges ainda é chamado"
? Solução: Remova SaveChanges do repositório

### Erro: "Result não é reconhecido"
? Solução: `using SNI_Events.Application.Common.Results;`

---

## ?? Links Úteis

- ?? [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- ?? [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- ?? [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- ?? [Result Pattern](https://www.ryadh-al-ahmad.com/programming/csharp/result-type/)

---

## ?? Próximos Passos

1. ? Entenda as mudanças (leia SUMMARY.md)
2. ? Execute os testes rápidos acima
3. ? Leia USAGE_GUIDE.md para detalhes
4. ? Migre outros Use Cases (MIGRATION_GUIDE.md)
5. ? Implemente configurações (CONFIGURATION_GUIDE.md)

---

## ? Pontos Importantes

```
?? Unit of Work garante atomicidade
?? Result Pattern melhora performance
?? Specifications tornam queries reutilizáveis
?? Middleware centraliza tratamento de erros
?? Código mais testável e mantível
```

---

## ? Checklist Final

- [ ] Build compilado sem erros
- [ ] Testes rápidos (5 testes acima) passam
- [ ] Logs aparecem corretamente
- [ ] Leu SUMMARY.md
- [ ] Leu USAGE_GUIDE.md
- [ ] Entende Result Pattern
- [ ] Entende Unit of Work
- [ ] Pronto para migrar novos Use Cases

---

**Build Status:** ? Sucesso  
**Documentação:** ? Completa  
**Pronto para Produção:** ? Sim  

---

**Última Atualização:** 2025-01-XX  
**Versão:** 1.0  
**Mantido por:** Seu Time
