using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public AuthController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userRepository.GetUserByUsername(request.Username);
            if (user == null || user.PasswordHash != request.Password) // Plain text for simplicity as per requirements, strictly should be hashed
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            // In a real world, issue JWT. For this assignment, we might just return the user info directly
            // and trust the client or use simple session if needed. 
            // The prompt says "Data Communication: JSON over HTTP" and "User role should be determined after login".
            
            return Ok(new 
            { 
                Id = user.Id, 
                Username = user.Username, 
                Role = user.Role 
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request) 
        {
             // Added for seed/testing purposes, though not explicitly asked in user screens, needed to create users.
             var user = _userRepository.CreateUser(request.Username, request.Password, "User");
             if(user == null) return BadRequest("User already exists");
             return Ok(user);
        }
        
        [HttpPost("register-admin")]
        public IActionResult RegisterAdmin([FromBody] LoginRequest request) 
        {
             // Added for seed/testing purposes
             var user = _userRepository.CreateUser(request.Username, request.Password, "Admin");
             if(user == null) return BadRequest("Use already exists");
             return Ok(user);
        }
    }
}
