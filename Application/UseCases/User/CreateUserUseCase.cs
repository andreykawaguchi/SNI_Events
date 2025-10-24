using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Services;
using SNI_Events.Domain.Specifications;

namespace SNI_Events.Application.UseCases.User
{
    /// <summary>
    /// Use Case para criar um novo usu�rio
    /// Implementa o padr�o Use Case da arquitetura limpa
    /// Responsabilidade �nica: coordenar a cria��o de usu�rio
    /// </summary>
    public class CreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDomainService _userDomainService;

        public CreateUserUseCase(IUserRepository userRepository, IUserDomainService userDomainService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        }

        public async Task<Domain.Entities.User> ExecuteAsync(CreateUserRequest request, long? createdByUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Validar CPF
            if (!_userDomainService.IsValidCpf(request.CPF))
                throw new ArgumentException("CPF inv�lido.", nameof(request.CPF));

            // Validar Email
            if (!_userDomainService.IsValidEmail(request.Email))
                throw new ArgumentException("Email inv�lido.", nameof(request.Email));

            // Verificar se CPF j� existe
            if (await _userDomainService.CpfAlreadyExistsAsync(request.CPF))
                throw new InvalidOperationException("CPF j� registrado no sistema.");

            // Verificar se Email j� existe
            if (await _userDomainService.EmailAlreadyExistsAsync(request.Email))
                throw new InvalidOperationException("Email j� registrado no sistema.");

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

            return user;
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
