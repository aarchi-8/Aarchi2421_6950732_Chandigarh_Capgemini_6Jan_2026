using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using System.Security.Claims;

namespace SmartHealthcare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        // GET: api/patients
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        // GET: api/patients/{id:int}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound(new { message = $"Patient with id {id} not found" });

            return Ok(patient);
        }

        // GET: api/patients/my-profile
        [HttpGet("my-profile")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
                return NotFound(new { message = "Patient profile not found" });

            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create([FromBody] CreatePatientDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var patient = await _patientService.CreatePatientAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = patient.PatientId }, patient);
        }

        // PUT: api/patients/{id:int}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _patientService.UpdatePatientAsync(id, dto);
            if (patient == null)
                return NotFound(new { message = $"Patient with id {id} not found" });

            return Ok(patient);
        }

        // PATCH: api/patients/{id:int}/medical-history
        [HttpPatch("{id:int}/medical-history")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateMedicalHistory(int id, [FromBody] string medicalHistory)
        {
            var dto = new UpdatePatientDTO { MedicalHistory = medicalHistory };
            var patient = await _patientService.UpdatePatientAsync(id, dto);
            if (patient == null)
                return NotFound(new { message = $"Patient with id {id} not found" });

            return Ok(patient);
        }

        // DELETE: api/patients/{id:int}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
                return NotFound(new { message = $"Patient with id {id} not found" });

            return Ok(new { message = "Patient deleted successfully" });
        }
    }
}