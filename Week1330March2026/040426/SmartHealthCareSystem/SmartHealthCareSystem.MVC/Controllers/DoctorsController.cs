using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApiService _apiService;

        public DoctorsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
            return View(doctors);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDoctorDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync();
                return View(model);
            }

            await _apiService.PostAsync<DoctorDto>("doctors", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _apiService.GetAsync<DoctorDto>($"doctors/{id}");
            if (doctor == null)
                return NotFound();

            await PopulateDepartmentsAsync();
            var updateModel = new UpdateDoctorDto
            {
                UserId = doctor.UserId,
                DepartmentId = doctor.DepartmentId,
                Specialization = doctor.Specialization,
                ExperienceYears = doctor.ExperienceYears,
                Availability = doctor.Availability
            };

            ViewBag.DoctorId = id;
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDoctorDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync();
                return View(model);
            }

            await _apiService.PutAsync($"doctors/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _apiService.GetAsync<DoctorDto>($"doctors/{id}");
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync($"doctors/{id}");
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDepartmentsAsync()
        {
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("departments");
            ViewBag.Departments = (departments ?? new List<DepartmentDto>()).Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToList();
        }
    }
}
