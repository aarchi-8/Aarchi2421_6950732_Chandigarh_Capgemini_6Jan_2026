using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApiService _apiService;

        public PatientsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            var patients = await _apiService.GetAsync<List<PatientDto>>("patients");
            if (!string.IsNullOrWhiteSpace(search))
                patients = patients.Where(p => p.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            return View(patients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _apiService.GetAsync<PatientDto>($"patients/{id}");
            if (patient == null) return NotFound();
            // Optionally fetch history (appointments, prescriptions, bills)
            return View(patient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _apiService.PostAsync<PatientDto>("patients", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _apiService.GetAsync<PatientDto>($"patients/{id}");
            if (patient == null) return NotFound();
            var updateModel = new UpdatePatientDto
            {
                FullName = patient.FullName,
                Email = patient.Email,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address
            };
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePatientDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _apiService.PutAsync($"patients/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _apiService.GetAsync<PatientDto>($"patients/{id}");
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync($"patients/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
