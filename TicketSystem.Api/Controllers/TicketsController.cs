using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly TicketRepository _ticketRepository;
        private readonly UserRepository _userRepository;

        public TicketsController(TicketRepository ticketRepository, UserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreateTicketRequest request)
        {
            // Simple validation
            if (string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Description))
                return BadRequest("Subject and Description are required");

            var ticket = new Ticket
            {
                Subject = request.Subject,
                Description = request.Description,
                Priority = request.Priority,
                CreatedByUserId = request.UserId // In real app, from Claims
            };

            var createdTicket = _ticketRepository.CreateTicket(ticket);
            return Ok(createdTicket);
        }

        [HttpGet]
        public IActionResult GetTickets([FromQuery] int? userId) // If userId present, filter by user; else (if admin) get all
        {
            // In real app, we check the role of the caller. 
            // For now, we trust the param, or better:
            // If the user is an admin (role check via another header/param), show all.
            // If user is normal user, show only theirs.
            // For this assignment, allowing userId to filter.
            
            var tickets = _ticketRepository.GetTickets(userId);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public IActionResult GetTicket(int id)
        {
            var ticket = _ticketRepository.GetTicketById(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet("{id}/history")]
        public IActionResult GetTicketHistory(int id)
        {
            var history = _ticketRepository.GetTicketHistory(id);
            return Ok(history);
        }

        [HttpPost("comment")]
        public IActionResult AddComment([FromBody] AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comment)) return BadRequest("Comment cannot be empty");
            _ticketRepository.AddComment(request.TicketId, request.Comment, request.UserId);
            return Ok();
        }

        [HttpPost("assign")]
        public IActionResult AssignTicket([FromBody] AssignRequest request)
        {
            // In a real app we would get the AdminId from the context if we wanted to log who did it as 'ChangedBy', 
            // but here the request payload contains target AdminId. 
            // The 'AssignTicket' method in Repo logs it as valid comment.
            _ticketRepository.AssignTicket(request.TicketId, request.AdminId);
            return Ok();
        }

        [HttpPost("status")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
             // We need to know WHO changed the status to log it in history properly.
             // Adding ChangedByUserId to the request DTO.
             _ticketRepository.UpdateStatus(request.TicketId, request.Status, request.UserId);
             return Ok();
        }
    }

    public class AssignRequest { public int TicketId { get; set; } public int AdminId { get; set; } }
    public class UpdateStatusRequest { public int TicketId { get; set; } public string Status { get; set; } public int UserId { get; set; } }
    public class AddCommentRequest { public int TicketId { get; set; } public string Comment { get; set; } public int UserId { get; set; } }
}
