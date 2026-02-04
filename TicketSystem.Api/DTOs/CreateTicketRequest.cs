namespace TicketSystem.Api.DTOs
{
    public class CreateTicketRequest
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public int UserId { get; set; } // In a real app, this would come from JWT claims
    }
}
