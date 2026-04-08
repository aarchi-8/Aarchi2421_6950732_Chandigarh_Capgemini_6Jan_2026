using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Web.Services;
using System.Dynamic;
using System.Text.Json;
using System.Collections;

namespace SmartHealthcare.Web.Controllers
{
    [Authorize(Roles = "Doctor,Admin")]
    public class PatientsController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IApiClient apiClient, ILogger<PatientsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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
                    
                    // Try to get patients from API
                    var response = await client.GetAsync("patients");
                    _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response Content Length: {Length}", content.Length);
                        
                        using (var doc = JsonDocument.Parse(content))
                        {
                            var root = doc.RootElement;
                            var patientList = new List<dynamic>();
                            
                            // Handle if response is a direct array
                            if (root.ValueKind == JsonValueKind.Array)
                            {
                                _logger.LogInformation("Response is direct array with {Count} items", root.GetArrayLength());
                                
                                foreach (var patientElement in root.EnumerateArray())
                                {
                                    dynamic patient = new ExpandoObject();
                                    var dict = (IDictionary<string, object?>)patient;
                                    
                                    if (patientElement.TryGetProperty("patientId", out var patientId))
                                        dict["patientId"] = patientId.GetInt32();
                                    if (patientElement.TryGetProperty("userId", out var userId))
                                        dict["userId"] = userId.GetInt32();
                                    if (patientElement.TryGetProperty("fullName", out var fullName))
                                        dict["fullName"] = fullName.GetString();
                                    if (patientElement.TryGetProperty("email", out var email))
                                        dict["email"] = email.GetString();
                                    if (patientElement.TryGetProperty("phone", out var phone))
                                        dict["phone"] = phone.GetString();
                                    if (patientElement.TryGetProperty("dateOfBirth", out var dob))
                                        dict["dateOfBirth"] = dob.GetString();
                                    if (patientElement.TryGetProperty("gender", out var gender))
                                        dict["gender"] = gender.GetString();
                                    if (patientElement.TryGetProperty("address", out var address))
                                        dict["address"] = address.GetString();
                                    
                                    patientList.Add(patient);
                                }
                                
                                _logger.LogInformation("Loaded {Count} patients total", patientList.Count);
                            }
                            
                            return View(patientList);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("API returned status {StatusCode}", response.StatusCode);
                        // Return empty list if API is not available yet
                        return View(new List<dynamic>());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patients: {Message}", ex.Message);
                // Return empty list instead of error
                return View(new List<dynamic>());
            }
        }

        public async Task<IActionResult> Details(int id)
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
                    
                    var response = await client.GetAsync($"patients/{id}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        using (var doc = JsonDocument.Parse(content))
                        {
                            var root = doc.RootElement;
                            dynamic patient = new ExpandoObject();
                            var dict = (IDictionary<string, object?>)patient;
                            
                            if (root.TryGetProperty("patientId", out var patientId))
                                dict["patientId"] = patientId.GetInt32();
                            if (root.TryGetProperty("userId", out var userId))
                                dict["userId"] = userId.GetInt32();
                            if (root.TryGetProperty("fullName", out var fullName))
                                dict["fullName"] = fullName.GetString();
                            if (root.TryGetProperty("email", out var email))
                                dict["email"] = email.GetString();
                            if (root.TryGetProperty("phone", out var phone))
                                dict["phone"] = phone.GetString();
                            if (root.TryGetProperty("dateOfBirth", out var dob))
                                dict["dateOfBirth"] = dob.GetString();
                            if (root.TryGetProperty("gender", out var gender))
                                dict["gender"] = gender.GetString();
                            if (root.TryGetProperty("address", out var address))
                                dict["address"] = address.GetString();
                            
                            return View(patient);
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Patient not found";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patient {Id}", id);
                TempData["Error"] = "Error loading patient: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
