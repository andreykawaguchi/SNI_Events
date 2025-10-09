namespace SNI_Events.Application.Dtos.User
{
    public class UserFilterDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
