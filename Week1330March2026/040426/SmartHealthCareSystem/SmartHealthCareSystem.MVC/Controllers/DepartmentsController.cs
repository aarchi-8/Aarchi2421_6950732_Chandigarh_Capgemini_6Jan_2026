using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApiService _apiService;

        public DepartmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("departments");
            return View(departments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _apiService.PostAsync<DepartmentDto>("departments", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _apiService.GetAsync<DepartmentDto>($"departments/{id}");
            if (department == null)
                return NotFound();

            var updateModel = new UpdateDepartmentDto
            {
                DepartmentName = department.DepartmentName,
                Description = department.Description
            };

            ViewBag.DepartmentId = id;
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDepartmentDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _apiService.PutAsync($"departments/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var department = await _apiService.GetAsync<DepartmentDto>($"departments/{id}");
            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync($"departments/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
