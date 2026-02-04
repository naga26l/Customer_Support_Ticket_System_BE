using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.Data;

namespace TicketSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("admins")]
        public IActionResult GetAdmins()
        {
            var admins = _userRepository.GetAdmins();
            return Ok(admins);
        }
    }
}
