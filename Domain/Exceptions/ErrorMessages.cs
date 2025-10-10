namespace SNI_Events.Domain.Exceptions
{
    public static class ErrorMessages
    {
        public const string EventNotFound = "Evento não encontrado.";
        public const string UserNotFound = "Usuário não encontrado.";
        public const string DinnerNotFound = "Jantar não encontrado.";
        public const string EntityNotBase = "A entidade não herda de EntityBase.";
        public const string EntityNotAuditable = "A entidade não herda de EntityBase e não pode ser auditada.";
        public const string UserNotAuthenticated = "Usuário não autenticado.";
        public const string CpfAlreadyRegistered = "CPF já cadastrado.";
        public const string EmailAlreadyRegistered = "E-mail já cadastrado.";
        public const string EmailCannotBeChanged = "E-mail não pode ser alterado.";
        public const string ArgumentNull = "Argumento nulo ou inválido.";
    }
}
