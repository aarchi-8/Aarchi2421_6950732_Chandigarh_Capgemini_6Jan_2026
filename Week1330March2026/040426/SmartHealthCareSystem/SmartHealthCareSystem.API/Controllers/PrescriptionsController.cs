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
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMapper _mapper;

        public PrescriptionsController(IPrescriptionService prescriptionService, IMapper mapper)
        {
            _prescriptionService = prescriptionService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetPrescriptions()
        {
            var prescriptionDtos = await _prescriptionService.GetAllPrescriptionsAsync();
            return Ok(prescriptionDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescription(int id)
        {
            var prescriptionDto = await _prescriptionService.GetPrescriptionByIdAsync(id);
            if (prescriptionDto == null)
                return NotFound();

            return Ok(prescriptionDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto createPrescriptionDto)
        {
            var prescriptionDto = await _prescriptionService.CreatePrescriptionAsync(createPrescriptionDto);
            return CreatedAtAction(nameof(GetPrescription), new { id = prescriptionDto.PrescriptionId }, prescriptionDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto updatePrescriptionDto)
        {
            var prescriptionDto = await _prescriptionService.UpdatePrescriptionAsync(id, updatePrescriptionDto);
            if (prescriptionDto == null)
                return NotFound();

            return Ok(prescriptionDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
