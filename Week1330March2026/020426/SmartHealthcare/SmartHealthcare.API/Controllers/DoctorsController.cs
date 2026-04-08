using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        // GET: api/doctors/search?pageNumber=1&pageSize=10&searchTerm=cardio&specializationId=1
        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] DoctorFilterParams filterParams)
        {
            var result = await _doctorService.GetDoctorsWithPaginationAsync(filterParams);
            return Ok(result);
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        // GET: api/doctors/{id:int}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = $"Doctor with id {id} not found" });

            return Ok(doctor);
        }

        // GET: api/doctors/specialization/{specializationId:int}
        [HttpGet("specialization/{specializationId:int}")]
        public async Task<IActionResult> GetBySpecialization(int specializationId)
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specializationId);
            return Ok(doctors);
        }

        // POST: api/doctors
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var doctor = await _doctorService.CreateDoctorAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = doctor.DoctorId }, doctor);
        }

        // PUT: api/doctors/{id:int}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await _doctorService.UpdateDoctorAsync(id, dto);
            if (doctor == null)
                return NotFound(new { message = $"Doctor with id {id} not found" });

            return Ok(doctor);
        }

        // PATCH: api/doctors/{id:int}/availability
        [HttpPatch("{id:int}/availability")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] bool isAvailable)
        {
            var dto = new UpdateDoctorDTO { IsAvailable = isAvailable };
            var doctor = await _doctorService.UpdateDoctorAsync(id, dto);
            if (doctor == null)
                return NotFound(new { message = $"Doctor with id {id} not found" });

            return Ok(doctor);
        }

        // DELETE: api/doctors/{id:int}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound(new { message = $"Doctor with id {id} not found" });

            return Ok(new { message = "Doctor deleted successfully" });
        }
    }
}
