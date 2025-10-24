namespace SNI_Events.Application.Dtos.Responses
{
    /// <summary>
    /// DTO para resposta padr�o da API
    /// Implementa o padr�o de resposta consistente
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        private ApiResponse() { }

        public static ApiResponse<T> Ok(T data, string message = "Opera��o realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = Array.Empty<string>()
            };
        }

        public static ApiResponse<T> Error(string message, IEnumerable<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? Array.Empty<string>()
            };
        }

        public static ApiResponse<T> Error(string message, string error)
        {
            return Error(message, new[] { error });
        }
    }

    /// <summary>
    /// DTO para resposta padr�o da API sem dados
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }

        private ApiResponse() { }

        public static ApiResponse Ok(string message = "Opera��o realizada com sucesso")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Errors = Array.Empty<string>()
            };
        }

        public static ApiResponse Error(string message, IEnumerable<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? Array.Empty<string>()
            };
        }

        public static ApiResponse Error(string message, string error)
        {
            return Error(message, new[] { error });
        }
    }
}
