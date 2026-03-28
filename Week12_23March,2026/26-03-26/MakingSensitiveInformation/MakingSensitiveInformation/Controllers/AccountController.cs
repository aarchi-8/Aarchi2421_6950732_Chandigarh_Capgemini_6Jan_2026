using Microsoft.AspNetCore.Mvc;

namespace MakingSensitiveInformation.Controllers
{
    using AutoMapper;
    using MakingSensitiveInformation.DTOs;
    using MakingSensitiveInformation.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;

        public AccountController(IMapper mapper)
        {
            _mapper = mapper;
        }

        // 🔥 API: /api/account/masked
        [HttpGet("masked")]
        public IActionResult GetMaskedAccount()
        {
            // Dummy data (replace with DB later)
            var account = new Account
            {
                Id = 1,
                AccountNumber = "1234567890"
            };

            var result = _mapper.Map<AccountDTO>(account);

            return Ok(result);
        }
    }
}
