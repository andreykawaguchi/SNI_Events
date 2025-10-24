namespace SNI_Events.Application.Common.Results
{
    /// <summary>
    /// Result padrão para operações que podem falhar
    /// Implementa o padrão Result para evitar exceções de negócio
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Error { get; protected set; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(true, value, string.Empty);
        public static Result<T> Failure<T>(string error) => new(false, default, error);
    }

    /// <summary>
    /// Result genérico que carrega um valor
    /// </summary>
    public class Result<T> : Result
    {
        public T Value { get; }

        public Result(bool isSuccess, T value, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
