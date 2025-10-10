namespace SNI_Events.Domain.Exceptions
{
    public static class ErrorMessages
    {
        public const string EventNotFound = "Evento n�o encontrado.";
        public const string UserNotFound = "Usu�rio n�o encontrado.";
        public const string DinnerNotFound = "Jantar n�o encontrado.";
        public const string EntityNotBase = "A entidade n�o herda de EntityBase.";
        public const string EntityNotAuditable = "A entidade n�o herda de EntityBase e n�o pode ser auditada.";
        public const string UserNotAuthenticated = "Usu�rio n�o autenticado.";
        public const string CpfAlreadyRegistered = "CPF j� cadastrado.";
        public const string EmailAlreadyRegistered = "E-mail j� cadastrado.";
        public const string EmailCannotBeChanged = "E-mail n�o pode ser alterado.";
        public const string ArgumentNull = "Argumento nulo ou inv�lido.";
    }
}
