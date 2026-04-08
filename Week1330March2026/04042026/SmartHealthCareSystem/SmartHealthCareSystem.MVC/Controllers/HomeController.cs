using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHealthCareSystem.MVC.Models;
using SmartHealthCareSystem.MVC.Services;
using SmartHealthCareSystem.Shared.DTOs;

namespace SmartHealthCareSystem.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApiService _apiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(ApiService apiService, IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userName = HttpContext.Session.GetString("UserName");
        
        if (userRole == "Admin")
            return RedirectToAction("Dashboard", "Admin");
        else if (userRole == "Doctor")
            return RedirectToAction("DoctorDashboard");
        else if (userRole == "Patient")
            return RedirectToAction("PatientDashboard");
        
        return View();
    }

    public async Task<IActionResult> DoctorDashboard()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");
        var userName = HttpContext.Session.GetString("UserName");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        ViewBag.DoctorName = userName;

        try
        {
            var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
            var doctor = doctors?.FirstOrDefault(d => d.UserId == userId.Value);
            if (doctor != null)
            {
                var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/doctor/{doctor.DoctorId}");
                ViewBag.Appointments = appointments?.OrderBy(a => a.AppointmentDate).ToList() ?? new List<AppointmentDto>();
            }
            else
            {
                ViewBag.Appointments = new List<AppointmentDto>();
            }
        }
        catch
        {
            ViewBag.Appointments = new List<AppointmentDto>();
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DoctorProfile()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
        var doctor = doctors?.FirstOrDefault(d => d.UserId == userId.Value);
        if (doctor == null)
            return RedirectToAction("CreateDoctorProfile");

        return View(doctor);
    }

    [HttpGet]
    public async Task<IActionResult> CreateDoctorProfile()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");
        var userName = HttpContext.Session.GetString("UserName");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        await PopulateDepartmentsAsync();
        ViewBag.FullName = userName;
        ViewBag.Email = userEmail;
        return View(new CreateDoctorDto { UserId = userId.Value });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDoctorProfile(CreateDoctorDto model)
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Doctor")
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync();
            return View(model);
        }

        await _apiService.PostAsync<DoctorDto>("doctors", model);
        return RedirectToAction("DoctorProfile");
    }

    [HttpGet]
    public async Task<IActionResult> EditDoctorProfile()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
        var doctor = doctors?.FirstOrDefault(d => d.UserId == userId.Value);
        if (doctor == null)
            return RedirectToAction("CreateDoctorProfile");

        await PopulateDepartmentsAsync();
        ViewBag.DoctorId = doctor.DoctorId;
        ViewBag.FullName = doctor.FullName;
        ViewBag.Email = doctor.Email;

        var updateModel = new UpdateDoctorDto
        {
            DepartmentId = doctor.DepartmentId,
            Specialization = doctor.Specialization,
            ExperienceYears = doctor.ExperienceYears,
            Availability = doctor.Availability,
            UserId = doctor.UserId
        };

        return View(updateModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDoctorProfile(int id, UpdateDoctorDto model)
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync();
            ViewBag.DoctorId = id;
            return View(model);
        }

        await _apiService.PutAsync($"doctors/{id}", model);
        TempData["SuccessMessage"] = "Doctor profile updated successfully.";
        return RedirectToAction("DoctorProfile");
    }

    private async Task PopulateDepartmentsAsync()
    {
        var departments = await _apiService.GetAsync<List<DepartmentDto>>("departments");
        ViewBag.Departments = departments?
            .Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString()))
            .ToList() ?? new List<SelectListItem>();
    }

    public async Task<IActionResult> PatientDashboard()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");
        var userName = HttpContext.Session.GetString("UserName");

        if (userRole != "Patient" || !userId.HasValue)
            return Unauthorized();

        ViewBag.PatientName = userName;

        try
        {
            var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/patient/{userId.Value}");
            ViewBag.UpcomingAppointments = appointments?
                .Where(a => a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .Take(6)
                .ToList() ?? new List<AppointmentDto>();

            var bills = await _apiService.GetAsync<List<BillDto>>($"bills/patient/{userId.Value}");
            ViewBag.OutstandingBills = bills?.Where(b => b.PaymentStatus == "Unpaid").ToList() ?? new List<BillDto>();
            ViewBag.PaidBills = bills?.Where(b => b.PaymentStatus == "Paid").ToList() ?? new List<BillDto>();
        }
        catch
        {
            ViewBag.UpcomingAppointments = new List<AppointmentDto>();
            ViewBag.OutstandingBills = new List<BillDto>();
            ViewBag.PaidBills = new List<BillDto>();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PayBills()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Patient" || !userId.HasValue)
            return Unauthorized();

        try
        {
            // Fix CS8625: pass an empty object instead of null
            await _apiService.PostAsync<object>($"bills/pay/{userId.Value}", new { });
            TempData["SuccessMessage"] = "Payment processed successfully! All outstanding bills have been marked as paid.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Failed to process payment. Please try again.";
        }

        return RedirectToAction("PatientDashboard");
    }

    public async Task<IActionResult> BookAppointment()
    {
        var userRole = HttpContext.Session.GetString("UserRole");

        if (userRole != "Patient")
            return Unauthorized();

        var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
        ViewBag.Doctors = doctors;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BookAppointment(CreateAppointmentDto model)
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Patient" || !userId.HasValue)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
            ViewBag.Doctors = doctors;
            return View(model);
        }

        model.PatientId = userId.Value;

        try
        {
            await _apiService.PostAsync<AppointmentDto>("appointments", model);
            TempData["SuccessMessage"] = "Appointment booked successfully! You will receive a confirmation soon.";
            return RedirectToAction("PatientDashboard");
        }
        catch
        {
            ModelState.AddModelError("", "Failed to book appointment. Please try again.");
            var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
            ViewBag.Doctors = doctors;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyPatients()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
        var doctor = doctors?.FirstOrDefault(d => d.UserId == userId.Value);
        if (doctor == null)
            return NotFound();

        var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/doctor/{doctor.DoctorId}");
        return View(appointments?.OrderBy(a => a.PatientName).ToList() ?? new List<AppointmentDto>());
    }

    [HttpGet]
    public async Task<IActionResult> ManageSchedule()
    {
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userRole != "Doctor" || !userId.HasValue)
            return Unauthorized();

        var doctors = await _apiService.GetAsync<List<DoctorDto>>("doctors");
        var doctor = doctors?.FirstOrDefault(d => d.UserId == userId.Value);
        if (doctor == null)
            return NotFound();

        var appointments = await _apiService.GetAsync<List<AppointmentDto>>($"appointments/doctor/{doctor.DoctorId}");
        return View(appointments?.OrderBy(a => a.AppointmentDate).ToList() ?? new List<AppointmentDto>());
    }

    public IActionResult WritePrescription()
    {
        return RedirectToAction("Create", "Prescriptions");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}