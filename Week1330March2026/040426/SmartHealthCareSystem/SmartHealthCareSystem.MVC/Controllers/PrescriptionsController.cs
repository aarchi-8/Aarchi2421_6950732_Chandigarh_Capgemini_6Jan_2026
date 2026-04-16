using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly ApiService _apiService;

        public PrescriptionsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");
            
            List<PrescriptionDto> prescriptions;
            
            if (userRole == "Admin")
            {
                prescriptions = await _apiService.GetAsync<List<PrescriptionDto>>("prescriptions") ?? new List<PrescriptionDto>();
            }
            else if (userRole == "Doctor" && userId.HasValue)
            {
                // Get prescriptions for doctor's appointments
                var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors") ?? new List<DoctorDto>();
                var doctor = doctors.FirstOrDefault(d => d.UserId == userId.Value);
                if (doctor != null)
                {
                    var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/doctor/{doctor.DoctorId}") ?? new List<AppointmentDto>();
                    var appointmentIds = appointments.Select(a => a.AppointmentId).ToList();
                    var allPrescriptions = await _apiService.GetAsync<List<PrescriptionDto>>("prescriptions") ?? new List<PrescriptionDto>();
                    prescriptions = allPrescriptions.Where(p => appointmentIds.Contains(p.AppointmentId)).ToList();
                }
                else
                {
                    prescriptions = new List<PrescriptionDto>();
                }
            }
            else if (userRole == "Patient" && userId.HasValue)
            {
                // Get prescriptions for patient's appointments
                var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/patient/{userId.Value}") ?? new List<AppointmentDto>();
                var appointmentIds = appointments.Select(a => a.AppointmentId).ToList();
                var allPrescriptions = await _apiService.GetAsync<List<PrescriptionDto>>("prescriptions") ?? new List<PrescriptionDto>();
                prescriptions = allPrescriptions.Where(p => appointmentIds.Contains(p.AppointmentId)).ToList();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            
            return View(prescriptions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _apiService.GetAsync<PrescriptionDto>($"prescriptions/{id}");
            if (prescription == null)
                return NotFound();

            return View(prescription);
        }

        public async Task<IActionResult> Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Doctor" && userRole != "Admin")
            {
                return Unauthorized();
            }

            await PopulateAppointmentsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePrescriptionDto model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Doctor" && userRole != "Admin")
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await PopulateAppointmentsAsync();
                return View(model);
            }

            try
            {
                await _apiService.PostAsync<PrescriptionDto>("prescriptions", model);
                TempData["SuccessMessage"] = "Prescription created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create prescription: {ex.Message}");
                await PopulateAppointmentsAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Doctor" && userRole != "Admin")
            {
                return Unauthorized();
            }

            var prescription = await _apiService.GetAsync<PrescriptionDto>($"prescriptions/{id}");
            if (prescription == null)
                return NotFound();

            await PopulateAppointmentsAsync();
            
            var updateModel = new UpdatePrescriptionDto
            {
                AppointmentId = prescription.AppointmentId,
                Diagnosis = prescription.Diagnosis,
                Medicines = prescription.Medicines,
                Notes = prescription.Notes
            };

            ViewBag.PrescriptionId = id;
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePrescriptionDto model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Doctor" && userRole != "Admin")
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await PopulateAppointmentsAsync();
                ViewBag.PrescriptionId = id;
                return View(model);
            }

            try
            {
                await _apiService.PutAsync($"prescriptions/{id}", model);
                TempData["SuccessMessage"] = "Prescription updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update prescription: {ex.Message}");
                await PopulateAppointmentsAsync();
                ViewBag.PrescriptionId = id;
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return Unauthorized();
            }

            var prescription = await _apiService.GetAsync<PrescriptionDto>($"prescriptions/{id}");
            if (prescription == null)
                return NotFound();

            return View(prescription);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return Unauthorized();
            }

            try
            {
                await _apiService.DeleteAsync($"prescriptions/{id}");
                TempData["SuccessMessage"] = "Prescription deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to delete prescription.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateAppointmentsAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");
            
            List<AppointmentDto> appointments;
            
            if (userRole == "Admin")
            {
                appointments = await _apiService.GetAsync<List<AppointmentDto>>("appointments") ?? new List<AppointmentDto>();
            }
            else if (userRole == "Doctor" && userId.HasValue)
            {
                var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors") ?? new List<DoctorDto>();
                var doctor = doctors.FirstOrDefault(d => d.UserId == userId.Value);
                if (doctor != null)
                {
                    appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/doctor/{doctor.DoctorId}") ?? new List<AppointmentDto>();
                }
                else
                {
                    appointments = new List<AppointmentDto>();
                }
            }
            else
            {
                appointments = new List<AppointmentDto>();
            }
            
            ViewBag.Appointments = appointments.Select(a => new SelectListItem(
                $"#{a.AppointmentId} - {a.PatientName} with {a.DoctorName} ({a.AppointmentDate:MMM dd})", 
                a.AppointmentId.ToString()
            )).ToList();
        }
    }
}
