namespace SNI_Events.Domain.Interfaces.Services
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        //Perfil? Perfil { get; }

        long? SessionTime { get; }
    }
}
