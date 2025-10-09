namespace SNI_Events.Application.Dtos.Event
{
    public class DinnerDto
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
