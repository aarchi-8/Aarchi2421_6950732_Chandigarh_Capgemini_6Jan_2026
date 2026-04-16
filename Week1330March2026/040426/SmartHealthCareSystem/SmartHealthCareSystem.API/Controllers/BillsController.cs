using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Services;

namespace SmartHealthCareSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly IMapper _mapper;

        public BillsController(IBillService billService, IMapper mapper)
        {
            _billService = billService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBills()
        {
            var billDtos = await _billService.GetAllBillsAsync();
            return Ok(billDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBill(int id)
        {
            var billDto = await _billService.GetBillByIdAsync(id);
            if (billDto == null)
                return NotFound();

            return Ok(billDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillDto createBillDto)
        {
            var billDto = await _billService.CreateBillAsync(createBillDto);
            return CreatedAtAction(nameof(GetBill), new { id = billDto.BillId }, billDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] UpdateBillDto updateBillDto)
        {
            var billDto = await _billService.UpdateBillAsync(id, updateBillDto);
            if (billDto == null)
                return NotFound();

            return Ok(billDto);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetBillsByPatient(int patientId)
        {
            var billDtos = await _billService.GetBillsByPatientIdAsync(patientId);
            return Ok(billDtos);
        }

        [HttpPost("pay/{patientId}")]
        public async Task<IActionResult> PayBills(int patientId)
        {
            var result = await _billService.MarkBillsAsPaidAsync(patientId);
            if (!result)
                return BadRequest("Failed to process payment");

            return Ok("Payment processed successfully");
        }
    }
}
