using System;

namespace TicketSystem.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } // Low, Medium, High
        public string Status { get; set; }   // Open, In Progress, Closed
        public int CreatedByUserId { get; set; }
        public string CreatedByUsername { get; set; } // For display
        public int? AssignedToUserId { get; set; }
        public string AssignedToUsername { get; set; } // For display
        public DateTime CreatedAt { get; set; }
    }
}
