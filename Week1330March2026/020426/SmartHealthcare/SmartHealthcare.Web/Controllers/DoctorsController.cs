using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;
using SmartHealthcare.Web.Services;
using System.Text.Json;
using System.Collections;

namespace SmartHealthcare.Web.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IApiClient apiClient, ILogger<DoctorsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5125/api/");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    // Use simple /doctors endpoint instead of /doctors/search
                    var response = await client.GetAsync("doctors");
                    _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response Content Length: {Length}", content.Length);
                        
                        using (var doc = JsonDocument.Parse(content))
                        {
                            var root = doc.RootElement;
                            var doctorList = new List<dynamic>();
                            
                            // Handle if response is a direct array
                            if (root.ValueKind == JsonValueKind.Array)
                            {
                                _logger.LogInformation("Response is direct array with {Count} items", root.GetArrayLength());
                                
                                foreach (var doctorElement in root.EnumerateArray())
                                {
                                    dynamic doctor = new System.Dynamic.ExpandoObject();
                                    var dict = (IDictionary<string, object?>)doctor;
                                    
                                    if (doctorElement.TryGetProperty("doctorId", out var doctorId))
                                        dict["doctorId"] = doctorId.GetInt32();
                                    if (doctorElement.TryGetProperty("userId", out var userId))
                                        dict["userId"] = userId.GetInt32();
                                    if (doctorElement.TryGetProperty("fullName", out var fullName))
                                        dict["fullName"] = fullName.GetString();
                                    if (doctorElement.TryGetProperty("email", out var email))
                                        dict["email"] = email.GetString();
                                    if (doctorElement.TryGetProperty("phone", out var phone))
                                        dict["phone"] = phone.GetString();
                                    if (doctorElement.TryGetProperty("qualification", out var qualification))
                                        dict["qualification"] = qualification.GetString();
                                    if (doctorElement.TryGetProperty("experienceYears", out var experienceYears))
                                        dict["experienceYears"] = experienceYears.GetInt32();
                                    if (doctorElement.TryGetProperty("consultationFee", out var consultationFee))
                                        dict["consultationFee"] = consultationFee.GetDecimal();
                                    if (doctorElement.TryGetProperty("isAvailable", out var isAvailable))
                                        dict["isAvailable"] = isAvailable.GetBoolean();
                                    
                                    // Apply search filter if search term provided
                                    if (string.IsNullOrEmpty(searchTerm) || 
                                        (doctor?.fullName?.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                                    {
                                        doctorList.Add(doctor);
                                    }
                                }
                                
                                _logger.LogInformation("Loaded {Count} doctors total", doctorList.Count);
                            }
                            else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("items", out var itemsElement))
                            {
                                // Handle paginated response format
                                if (itemsElement.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var doctorElement in itemsElement.EnumerateArray())
                                    {
                                        dynamic doctor = new System.Dynamic.ExpandoObject();
                                        var dict = (IDictionary<string, object?>)doctor;
                                        
                                        if (doctorElement.TryGetProperty("doctorId", out var doctorId))
                                            dict["doctorId"] = doctorId.GetInt32();
                                        if (doctorElement.TryGetProperty("userId", out var userId))
                                            dict["userId"] = userId.GetInt32();
                                        if (doctorElement.TryGetProperty("fullName", out var fullName))
                                            dict["fullName"] = fullName.GetString();
                                        if (doctorElement.TryGetProperty("email", out var email))
                                            dict["email"] = email.GetString();
                                        if (doctorElement.TryGetProperty("phone", out var phone))
                                            dict["phone"] = phone.GetString();
                                        if (doctorElement.TryGetProperty("qualification", out var qualification))
                                            dict["qualification"] = qualification.GetString();
                                        if (doctorElement.TryGetProperty("experienceYears", out var experienceYears))
                                            dict["experienceYears"] = experienceYears.GetInt32();
                                        if (doctorElement.TryGetProperty("consultationFee", out var consultationFee))
                                            dict["consultationFee"] = consultationFee.GetDecimal();
                                        if (doctorElement.TryGetProperty("isAvailable", out var isAvailable))
                                            dict["isAvailable"] = isAvailable.GetBoolean();
                                        
                                        doctorList.Add(doctor);
                                    }
                                }
                            }
                            
                            // Create response object to pass to view
                            dynamic viewModel = new System.Dynamic.ExpandoObject();
                            var vmDict = (IDictionary<string, object?>)viewModel;
                            vmDict["items"] = doctorList;
                            vmDict["pageNumber"] = pageNumber;
                            vmDict["totalPages"] = 1;
                            vmDict["hasNextPage"] = false;
                            vmDict["hasPreviousPage"] = false;
                            
                            ViewBag.SearchTerm = searchTerm;
                            
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        _logger.LogError("API Error: {StatusCode}", response.StatusCode);
                        TempData["Error"] = $"Failed to fetch doctors: {response.StatusCode}";
                        return View(new System.Dynamic.ExpandoObject());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctors: {Message}", ex.Message);
                TempData["Error"] = "Failed to load doctors, showing sample data: " + ex.Message;

                var sampleDoctors = GetSampleDoctors();
                dynamic viewModel = new System.Dynamic.ExpandoObject();
                var vmDict = (IDictionary<string, object?>)viewModel;
                vmDict["items"] = sampleDoctors;
                vmDict["pageNumber"] = pageNumber;
                vmDict["totalPages"] = 1;
                vmDict["hasNextPage"] = false;
                vmDict["hasPreviousPage"] = false;
                ViewBag.SearchTerm = searchTerm;

                return View(viewModel);
            }
        }

        private static List<dynamic> GetSampleDoctors()
        {
            var sample = new List<dynamic>();

            void AddDoctor(int id, string fullName, string phone, string qualification, int exp, decimal fee, bool available)
            {
                dynamic doctor = new System.Dynamic.ExpandoObject();
                var dict = (IDictionary<string, object?>)doctor;
                dict["doctorId"] = id;
                dict["fullName"] = fullName;
                dict["phone"] = phone;
                dict["qualification"] = qualification;
                dict["experienceYears"] = exp;
                dict["consultationFee"] = fee;
                dict["isAvailable"] = available;
                sample.Add(doctor);
            }

            AddDoctor(1, "Dr. Priya Desai", "+91 98765 43210", "MBBS, MD", 11, 850, true);
            AddDoctor(2, "Dr. Rahul Mehta", "+91 91234 56789", "MBBS, DNB (Cardiology)", 17, 1200, true);
            AddDoctor(3, "Dr. Anjali Verma", "+91 99887 66554", "MBBS, MS (Orthopedics)", 9, 780, false);

            return sample;
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }

                _apiClient.SetAuthToken(token);
                var doctor = await _apiClient.GetAsync<dynamic>($"doctors/{id}");
                
                return View(doctor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Doctor not found: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
