namespace SNI_Events.Domain.Services
{
    /// <summary>
    /// Domain Service respons�vel pela l�gica de neg�cio do usu�rio
    /// Implementa o padr�o Strategy para separar regras de neg�cio da infraestrutura
    /// </summary>
    public interface IUserDomainService
    {
        /// <summary>
        /// Valida se um CPF j� est� registrado no sistema
        /// </summary>
        Task<bool> CpfAlreadyExistsAsync(string cpf);

        /// <summary>
        /// Valida se um Email j� est� registrado no sistema
        /// </summary>
        Task<bool> EmailAlreadyExistsAsync(string email);

        /// <summary>
        /// Verifica se um CPF � v�lido
        /// </summary>
        bool IsValidCpf(string cpf);

        /// <summary>
        /// Verifica se um Email � v�lido
        /// </summary>
        bool IsValidEmail(string email);
    }
}
