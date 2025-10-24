namespace SNI_Events.Application.Dtos.User
{
    public class UserCreateRequestDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string CPF { get; set; } = default!;
        public string Role { get; set; } = "User"; // Default role
    }
}
