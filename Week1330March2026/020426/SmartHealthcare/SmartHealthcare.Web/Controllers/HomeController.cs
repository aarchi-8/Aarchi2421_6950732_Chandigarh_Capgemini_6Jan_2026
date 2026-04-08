using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Web.Models;
using SmartHealthcare.Web.Services;

namespace SmartHealthcare.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IApiClient _apiClient;

    public HomeController(ILogger<HomeController> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Dashboard");

        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _apiClient.SetAuthToken(token);
            }

            var appointmentList = await _apiClient.GetAsync<List<AppointmentDTO>>("appointments/my");
            appointmentList ??= new List<AppointmentDTO>();

            var upcoming = appointmentList
                .Where(a => a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            ViewBag.UpcomingAppointments = upcoming;
            ViewBag.UpcomingCount = upcoming.Count;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching upcoming appointments for dashboard: {Error}", ex.Message);
            ViewBag.UpcomingAppointments = new List<AppointmentDTO>();
            ViewBag.Error = "Unable to load upcoming appointments right now.";
            return View();
        }
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
