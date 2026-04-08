using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService,
            IPatientRepository patientRepository,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _patientRepository = patientRepository;
            _logger = logger;
        }

        // GET: api/appointments
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        // GET: api/appointments/{id:int}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with id {id} not found" });

            return Ok(appointment);
        }

        // GET: api/appointments?date=2026-04-01
        [HttpGet("by-date")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var appointments = await _appointmentService.GetAppointmentsByDateAsync(date);
            return Ok(appointments);
        }

        // GET: api/appointments/patient/{patientId:int}
        [HttpGet("patient/{patientId:int}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
            return Ok(appointments);
        }

        // GET: api/appointments/my - Get current user's appointments
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null)
                {
                    _logger.LogError("userId claim not found in JWT token");
                    return BadRequest(new { message = "User ID not found in token" });
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogError("userId claim is not a valid integer: {UserId}", userIdClaim.Value);
                    return BadRequest(new { message = "Invalid user ID format in token" });
                }

                _logger.LogInformation("Fetching appointments for current user: {UserId}", userId);
                
                // Get patient ID from userId
                var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
                if (patient == null)
                {
                    _logger.LogWarning("No patient found for userId: {UserId}", userId);
                    return Ok(new List<AppointmentDTO>()); // Return empty list if no patient found
                }

                var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patient.PatientId);
                _logger.LogInformation("Found {Count} appointments for user {UserId}, patient {PatientId}", 
                    appointments.Count(), userId, patient.PatientId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my appointments: {Error}", ex.Message);
                return StatusCode(500, new { message = $"Error fetching appointments: {ex.Message}" });
            }
        }

        // GET: api/appointments/doctor/{doctorId:int}
        [HttpGet("doctor/{doctorId:int}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            return Ok(appointments);
        }

        // POST: api/appointments
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid appointment data", errors = ModelState.Values.SelectMany(v => v.Errors) });

            try
            {
                // Extract userId from JWT claims
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null)
                {
                    _logger.LogError("userId claim not found in JWT token. Claims: {Claims}", 
                        string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                    return BadRequest(new { message = "User ID not found in token. Please log out and log in again." });
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogError("userId claim is not a valid integer: {UserId}", userIdClaim.Value);
                    return BadRequest(new { message = "Invalid user ID format in token" });
                }

                _logger.LogInformation("Creating appointment for userId: {UserId}, doctorId: {DoctorId}", userId, dto.DoctorId);

                var appointment = await _appointmentService.CreateAppointmentAsync(userId, dto);
                _logger.LogInformation("Appointment created successfully with ID: {AppointmentId}", appointment.AppointmentId);
                return CreatedAtAction(nameof(GetById), new { id = appointment.AppointmentId }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment: {Message}", ex.Message);
                return StatusCode(500, new { message = $"Error creating appointment: {ex.Message}" });
            }
        }

        // PUT: api/appointments/{id:int}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentService.UpdateAppointmentAsync(id, dto);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with id {id} not found" });

            return Ok(appointment);
        }

        // PATCH: api/appointments/{id:int}/status
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            if (!Enum.TryParse<SmartHealthcare.Models.Enums.AppointmentStatus>(status, out var parsedStatus))
                return BadRequest(new { message = "Invalid status value" });

            var dto = new UpdateAppointmentDTO { Status = parsedStatus };
            var appointment = await _appointmentService.UpdateAppointmentAsync(id, dto);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with id {id} not found" });

            return Ok(appointment);
        }

        // DELETE: api/appointments/{id:int}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound(new { message = $"Appointment with id {id} not found" });

            return Ok(new { message = "Appointment deleted successfully" });
        }
    }
}