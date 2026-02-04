using System;

namespace TicketSystem.Desktop.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUsername { get; set; }
        public int? AssignedToUserId { get; set; }
        public string AssignedToUsername { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
