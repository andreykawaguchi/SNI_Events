namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Domain Service responsável pela lógica de negócio do usuário
    /// Implementa o padrão Strategy para separar regras de negócio da infraestrutura
    /// </summary>
    public interface IUserDomainService
    {
        /// <summary>
        /// Valida se um CPF já está registrado no sistema
        /// </summary>
        Task<bool> CpfAlreadyExistsAsync(string cpf);

        /// <summary>
        /// Valida se um Email já está registrado no sistema
        /// </summary>
        Task<bool> EmailAlreadyExistsAsync(string email);

        /// <summary>
        /// Verifica se um CPF é válido
        /// </summary>
        bool IsValidCpf(string cpf);

        /// <summary>
        /// Verifica se um Email é válido
        /// </summary>
        bool IsValidEmail(string email);
    }
}
