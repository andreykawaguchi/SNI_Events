namespace SNI_Events.Application.Dtos.User
{
    public class UserUpdateRequestDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string CPF { get; set; } = default!;
        public string Role { get; set; } = "User"; // Default role
    }
}
