using System;

namespace TicketSystem.Api.Models
{
    public class TicketHistoryItem
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Comment" or "StatusChange" or "Assignment"
        public string Content { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
