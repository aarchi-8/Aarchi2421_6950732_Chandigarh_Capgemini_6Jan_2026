using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureAccountDetails.Data;
using SecureAccountDetails.Mapping;
using System.Security.Claims;

namespace SecureAccountDetails.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize]  // 🔐 Only logged-in users
        [HttpGet("details")]
        public IActionResult GetAccountDetails()
        {
            // 🔑 Get UserId from JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            // 📦 Fetch account from DB
            var account = _context.Accounts
                .FirstOrDefault(a => a.UserId == userId);

            if (account == null)
                return NotFound("Account not found");

            // 🔄 Map Entity → DTO
            var accountDto = _mapper.Map<AccountDetailsDTO>(account);

            return Ok(accountDto);
        }
    }
}
