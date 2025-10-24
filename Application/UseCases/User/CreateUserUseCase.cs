using SNI_Events.Application.Common.Results;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Interfaces.Services;
using SNI_Events.Domain.Services;
using SNI_Events.Domain.Specifications;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para criar um novo usu�rio
    /// Implementa o padr�o Use Case da arquitetura limpa
    /// Responsabilidade �nica: coordenar a cria��o de usu�rio
    /// Utiliza Result Pattern para tratamento de erros
    /// Utiliza Unit of Work para controle transacional
    /// </summary>
    public class CreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserUseCase(
            IUserRepository userRepository, 
            IUserDomainService userDomainService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Domain.Entities.User>> ExecuteAsync(CreateUserRequest request, long? createdByUserId)
        {
            if (request == null)
                return Result.Failure<Domain.Entities.User>("Requisi��o inv�lida");

            // Validar CPF
            if (!_userDomainService.IsValidCpf(request.CPF))
                return Result.Failure<Domain.Entities.User>("CPF inv�lido");

            // Validar Email
            if (!_userDomainService.IsValidEmail(request.Email))
                return Result.Failure<Domain.Entities.User>("Email inv�lido");

            // Verificar se CPF j� existe
            if (await _userDomainService.CpfAlreadyExistsAsync(request.CPF))
                return Result.Failure<Domain.Entities.User>("CPF j� registrado no sistema");

            // Verificar se Email j� existe usando Specification
            var emailSpec = new UserByEmailSpecification(request.Email);
            var existingUser = await _userRepository.FindBySpecificationAsync(emailSpec);
            if (existingUser != null)
                return Result.Failure<Domain.Entities.User>("Email j� registrado no sistema");

            try
            {
                // Criar entidade de dom�nio
                var user = new Domain.Entities.User(
                    request.Name,
                    request.Email,
                    request.Password,
                    request.PhoneNumber,
                    request.CPF,
                    createdByUserId);

                // Persistir
                await _userRepository.AddAsync(user);

                // Confirmar transa��o
                await _unitOfWork.CommitAsync();

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<Domain.Entities.User>($"Erro ao criar usu�rio: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Request DTO para Use Case de cria��o de usu�rio
    /// </summary>
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string CPF { get; set; }
    }
}
