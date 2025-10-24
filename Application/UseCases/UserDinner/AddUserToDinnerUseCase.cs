using SNI_Events.Domain.Entities;
using SNI_Events.Domain.Enum;
using SNI_Events.Domain.Interfaces.Repositories;
using SNI_Events.Domain.Services;

namespace SNI_Events.Application.UseCases.UserDinner
{
    /// <summary>
    /// Use Case para adicionar um usu�rio a um jantar
    /// Implementa a l�gica de neg�cio usando Domain Service
    /// </summary>
    public class AddUserToDinnerUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDinnerRepository _dinnerRepository;
        private readonly IUserDinnerDomainService _userDinnerDomainService;

        public AddUserToDinnerUseCase(
            IUserRepository userRepository,
            IDinnerRepository dinnerRepository,
            IUserDinnerDomainService userDinnerDomainService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _dinnerRepository = dinnerRepository ?? throw new ArgumentNullException(nameof(dinnerRepository));
            _userDinnerDomainService = userDinnerDomainService ?? throw new ArgumentNullException(nameof(userDinnerDomainService));
        }

        public async Task<Domain.Entities.UserDinner> ExecuteAsync(long userId, long dinnerId, bool isPresent, EStatusPayment paymentStatus, long? createdByUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Usu�rio com ID {userId} n�o encontrado.");

            var dinner = await _dinnerRepository.GetByIdAsync(dinnerId)
                ?? throw new KeyNotFoundException($"Jantar com ID {dinnerId} n�o encontrado.");

            if (!_userDinnerDomainService.CanAddUserToDinner(user, dinner))
                throw new InvalidOperationException("Usu�rio j� est� adicionado a este jantar.");

            var userDinner = _userDinnerDomainService.CreateUserDinnerAssociation(
                user, dinner, isPresent, paymentStatus, createdByUserId);

            user.UserDinners.Add(userDinner);
            dinner.UserDinners.Add(userDinner);

            await _userRepository.UpdateAsync(user);

            return userDinner;
        }
    }
}
