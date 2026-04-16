using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Services;

namespace SmartHealthCareSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;

        public DoctorsController(IDoctorService doctorService, IMapper mapper)
        {
            _doctorService = doctorService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctors()
        {
            var doctorDtos = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctorDtos);
        }

        [HttpGet("by-department/{departmentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            var departmentDoctors = doctors.Where(d => d.DepartmentId == departmentId).ToList();
            return Ok(departmentDoctors);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctorDto = await _doctorService.GetDoctorByIdAsync(id);
            if (doctorDto == null)
                return NotFound();

            return Ok(doctorDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            var doctorDto = await _doctorService.CreateDoctorAsync(createDoctorDto);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctorDto.DoctorId }, doctorDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            var doctorDto = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
            if (doctorDto == null)
                return NotFound();

            return Ok(doctorDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
