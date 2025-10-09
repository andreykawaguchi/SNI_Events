using SNI_Events.Domain.Interfaces.Services;
using System.Security.Claims;

namespace SNI_Events.Domain.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (long.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }

        //public Perfil? Perfil
        //{
        //    get
        //    {
        //        var roleString = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //        if (Enum.TryParse<Perfil>(roleString, out var perfil))
        //        {
        //            return perfil;
        //        }
        //        return null;
        //    }
        //}

        public long? SessionTime
        {
            get
            {
                var timeString = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Dsa)?.Value;
                if (long.TryParse(timeString, out var time))
                {
                    return time;
                }
                return 60;
            }
        }
    }
}
