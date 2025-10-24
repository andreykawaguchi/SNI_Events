using SNI_Events.Application.Common.Results;

namespace SNI_Events.Application.UseCases.Base
{
    /// <summary>
    /// Classe base para Use Cases
    /// Define padr�o comum para opera��es que retornam Result
    /// </summary>
    public abstract class BaseUseCase
    {
        protected static Result<T> HandleError<T>(Exception ex, string userMessage = "Erro ao processar opera��o")
        {
            // Aqui voc� pode adicionar logging, tratamento espec�fico, etc.
            return Result.Failure<T>($"{userMessage}: {ex.Message}");
        }

        protected static async Task<Result<T>> ExecuteWithTransactionAsync<T>(
            Func<Task<T>> operation,
            string errorMessage = "Erro ao processar opera��o")
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
