using SNI_Events.Domain.Interfaces.Repositories;

namespace SNI_Events.Application.UseCases.UserDinner
{
    /// <summary>
    /// Use Case para atualizar a presença de um usuário em um jantar
    /// </summary>
    public class UpdateUserDinnerPresenceUseCase
    {
        private readonly IDinnerRepository _dinnerRepository;

        public UpdateUserDinnerPresenceUseCase(IDinnerRepository dinnerRepository)
        {
            _dinnerRepository = dinnerRepository ?? throw new ArgumentNullException(nameof(dinnerRepository));
        }

        public async Task ExecuteAsync(long dinnerId, long userDinnerId, bool isPresent, long? modifiedByUserId)
        {
            var dinner = await _dinnerRepository.GetByIdAsync(dinnerId)
                ?? throw new KeyNotFoundException($"Jantar com ID {dinnerId} não encontrado.");

            var userDinner = dinner.UserDinners.FirstOrDefault(ud => ud.Id == userDinnerId)
                ?? throw new KeyNotFoundException($"Registro de participação não encontrado.");

            userDinner.UpdatePresence(isPresent, modifiedByUserId);

            await _dinnerRepository.UpdateAsync(dinner);
        }
    }
}
