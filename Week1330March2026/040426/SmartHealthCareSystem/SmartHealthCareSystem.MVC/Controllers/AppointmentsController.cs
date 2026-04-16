using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApiService _apiService;

        public AppointmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
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
                // Get doctor's appointments
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
            else if (userRole == "Patient" && userId.HasValue)
            {
                appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/patient/{userId.Value}") ?? new List<AppointmentDto>();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            
            return View(appointments);
        }

        public async Task<IActionResult> Create()
        {
            await PopulatePatientsAsync();
            await PopulateDepartmentsAsync();
            await PopulateDoctorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePatientsAsync();
                await PopulateDoctorsAsync();
                return View(model);
            }

            await _apiService.PostAsync<AppointmentDto>("appointments", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _apiService.GetAsync<AppointmentDto>($"appointments/{id}");
            if (appointment == null)
                return NotFound();

            await PopulateDoctorsAsync();
            var updateModel = new UpdateAppointmentDto
            {
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status
            };

            ViewBag.AppointmentId = id;
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDoctorsAsync();
                return View(model);
            }

            await _apiService.PutAsync($"appointments/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _apiService.GetAsync<AppointmentDto>($"appointments/{id}");
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync($"appointments/{id}");
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDoctorsAsync()
        {
            var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
            ViewBag.Doctors = (doctors ?? new List<DoctorDto>()).Select(d => new SelectListItem(d.FullName, d.DoctorId.ToString())).ToList();
        }

        private async Task PopulateDepartmentsAsync()
        {
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("departments");
            ViewBag.Departments = (departments ?? new List<DepartmentDto>()).Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToList();
        }

        private async Task PopulatePatientsAsync()
        {
            var users = await _apiService.GetAsync<List<UserDto>>("users");
            var patients = (users ?? new List<UserDto>()).Where(u => u.Role == "Patient").ToList();
            ViewBag.Patients = patients.Select(p => new SelectListItem(p.FullName, p.UserId.ToString())).ToList();
        }
    }
}
