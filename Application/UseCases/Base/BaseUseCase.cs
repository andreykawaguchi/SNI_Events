using SNI_Events.Application.Common.Results;

namespace SNI_Events.Application.UseCases.Base
{
    /// <summary>
    /// Classe base para Use Cases
    /// Define padrão comum para operações que retornam Result
    /// </summary>
    public abstract class BaseUseCase
    {
        protected static Result<T> HandleError<T>(Exception ex, string userMessage = "Erro ao processar operação")
        {
            // Aqui você pode adicionar logging, tratamento específico, etc.
            return Result.Failure<T>($"{userMessage}: {ex.Message}");
        }

        protected static async Task<Result<T>> ExecuteWithTransactionAsync<T>(
            Func<Task<T>> operation,
            string errorMessage = "Erro ao processar operação")
        {
            try
            {
                var result = await operation();
                return Result.Success(result);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<T>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<T>(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleError<T>(ex, errorMessage);
            }
        }
    }
}
