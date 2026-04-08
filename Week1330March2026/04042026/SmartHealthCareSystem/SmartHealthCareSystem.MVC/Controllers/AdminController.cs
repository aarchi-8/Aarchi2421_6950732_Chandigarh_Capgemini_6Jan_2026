using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;
using System.Collections.Generic;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(ApiService apiService, IHttpContextAccessor httpContextAccessor) 
            : base(apiService, httpContextAccessor)
        {
        }

        // Check if user is admin - using base controller helper
        private IActionResult RequireAdmin()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            return new EmptyResult();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            ViewData["Title"] = "Admin Dashboard";
            return View();
        }

        // Helper method to fetch data with fallback
        private async Task<IEnumerable<T>> FetchWithFallbackAsync<T>(string endpoint) where T : class
        {
            try
            {
                var result = await _apiService.GetAsync<List<T>>(endpoint);
                return result ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        // Doctors Management
        [HttpGet]
        public async Task<IActionResult> ManageDoctors()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var doctors = await FetchWithFallbackAsync<DoctorDto>("doctors");
            return View(doctors);
        }

        // Users Management
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var users = await FetchWithFallbackAsync<UserDto>("users");
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto userDto)
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            if (!ModelState.IsValid)
                return View(userDto);

            try
            {
                await _apiService.PostAsync<object>("users", userDto);
                TempData["Success"] = "User created successfully";
                return RedirectToAction("ManageUsers");
            }
            catch
            {
                ModelState.AddModelError("", "Failed to create user");
                return View(userDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            try
            {
                await _apiService.DeleteAsync($"users/{id}");
                TempData["Success"] = "User deleted successfully";
            }
            catch
            {
                TempData["Error"] = "Failed to delete user";
            }

            return RedirectToAction("ManageUsers");
        }

        // Appointments Management
        [HttpGet]
        public async Task<IActionResult> ManageAppointments()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var appointments = await FetchWithFallbackAsync<AppointmentDto>("appointments");
            return View(appointments);
        }

        // Departments Management
        [HttpGet]
        public async Task<IActionResult> ManageDepartments()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var departments = await FetchWithFallbackAsync<DepartmentDto>("departments");
            return View(departments);
        }

        // Prescriptions Management
        [HttpGet]
        public async Task<IActionResult> ManagePrescriptions()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var prescriptions = await FetchWithFallbackAsync<PrescriptionDto>("prescriptions");
            return View(prescriptions);
        }

        // Bills Management
        [HttpGet]
        public async Task<IActionResult> ManageBills()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            var bills = await FetchWithFallbackAsync<BillDto>("bills");
            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBill()
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            await PopulateAppointmentsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill(CreateBillDto billDto)
        {
            var adminCheck = RequireAdmin();
            if (adminCheck is not EmptyResult)
                return adminCheck;

            if (!ModelState.IsValid)
            {
                await PopulateAppointmentsAsync();
                return View(billDto);
            }

            try
            {
                await _apiService.PostAsync<BillDto>("bills", billDto);
                TempData["Success"] = "Bill created successfully";
                return RedirectToAction("ManageBills");
            }
            catch
            {
                ModelState.AddModelError("", "Failed to create bill");
                await PopulateAppointmentsAsync();
                return View(billDto);
            }
        }

        private async Task PopulateAppointmentsAsync()
        {
            try
            {
                var appointments = await _apiService.GetAsync<List<AppointmentDto>>("appointments");
                ViewBag.Appointments = appointments ?? new List<AppointmentDto>();
            }
            catch
            {
                ViewBag.Appointments = new List<AppointmentDto>();
            }
        }
    }
}
