using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Web.Services;
using System.Collections;
using System.Text.Json;
using System.Dynamic;

namespace SmartHealthcare.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<AppointmentsController> _logger;
        public AppointmentsController(IApiClient apiClient, ILogger<AppointmentsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                _logger.LogInformation("Index called. Token exists: {TokenExists}", !string.IsNullOrEmpty(token));
                
                if (!string.IsNullOrEmpty(token))
                    _apiClient.SetAuthToken(token);

                // Use the new /my endpoint that gets current user's appointments
                var appointments = await _apiClient.GetAsync<IEnumerable<AppointmentDTO>>("appointments/my");
                _logger.LogInformation("Fetched {Count} appointments from API", appointments?.Count() ?? 0);
                
                return View(appointments ?? new List<AppointmentDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments: {Error}", ex.Message);
                ViewBag.Error = $"Could not fetch appointments: {ex.Message}";
                return View(new List<AppointmentDTO>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Book()
        {
            var doctorList = new List<ExpandoObject>();
            
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                _logger.LogInformation("Book appointment called. Token exists: {TokenExists}", !string.IsNullOrEmpty(token));
                
                if (!string.IsNullOrEmpty(token))
                    _apiClient.SetAuthToken(token);

                // Fetch doctors from API
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5125/api/");
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    _logger.LogInformation("Calling API endpoint: {Endpoint}", "http://localhost:5125/api/doctors");
                    var response = await client.GetAsync("doctors");
                    
                    _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("API response content length: {Length}", content.Length);
                        
                        // Parse JSON response
                        using (var doc = JsonDocument.Parse(content))
                        {
                            JsonElement arrayElement = doc.RootElement;
                            _logger.LogInformation("Root element type: {Type}", arrayElement.ValueKind);
                            
                            // If response has 'value' property (wrapped), extract it
                            if (doc.RootElement.ValueKind == JsonValueKind.Object && 
                                doc.RootElement.TryGetProperty("value", out var valueElement))
                            {
                                arrayElement = valueElement;
                                _logger.LogInformation("Extracted 'value' property from response");
                            }
                            
                            // Process array elements
                            if (arrayElement.ValueKind == JsonValueKind.Array)
                            {
                                int count = 0;
                                foreach (var doctorElement in arrayElement.EnumerateArray())
                                {
                                    count++;
                                    dynamic doctor = new ExpandoObject();
                                    var dict = (IDictionary<string, object?>)doctor;
                                    
                                    if (doctorElement.TryGetProperty("doctorId", out var doctorId))
                                        dict["doctorId"] = doctorId.GetInt32();
                                    if (doctorElement.TryGetProperty("fullName", out var fullName))
                                        dict["fullName"] = fullName.GetString();
                                    if (doctorElement.TryGetProperty("specializations", out var specs))
                                    {
                                        var specList = new List<string>();
                                        foreach (var spec in specs.EnumerateArray())
                                        {
                                            specList.Add(spec.GetString() ?? "General");
                                        }
                                        dict["specializations"] = specList;
                                    }
                                    
                                    doctorList.Add(doctor);
                                    _logger.LogInformation("Added doctor: {DoctorId} - {FullName}", dict["doctorId"], dict["fullName"]);
                                }
                                _logger.LogInformation("Total doctors loaded: {Count}", count);
                            }
                            else
                            {
                                _logger.LogWarning("Response element is not an array. Type: {Type}", arrayElement.ValueKind);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("API call failed with status {StatusCode}", response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors: {Error}", ex.Message);
            }

            ViewBag.Doctors = doctorList;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create(CreateAppointmentDTO model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.LogWarning("ModelState invalid. Errors: {Errors}", string.Join("; ", errors.Select(e => e.ErrorMessage)));
                ViewBag.Error = "Please fill in all required fields";
                return View("Book", model);
            }

            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                _logger.LogInformation("Create appointment called. Token exists: {TokenExists}", !string.IsNullOrEmpty(token));
                _logger.LogInformation("Booking appointment: DoctorId={DoctorId}, Date={Date}, Reason={Reason}", 
                    model.DoctorId, model.AppointmentDate, model.Reason);
                
                if (string.IsNullOrEmpty(token))
                {
                    ViewBag.Error = "Session expired. Please log in again.";
                    return View("Book", model);
                }

                _apiClient.SetAuthToken(token);

                // Make the API call
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5125/api/");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    var json = System.Text.Json.JsonSerializer.Serialize(model);
                    _logger.LogInformation("Sending to API: {Json}", json);
                    
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("appointments", content);
                    
                    _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("API response content: {Content}", responseContent);
                        
                        var result = System.Text.Json.JsonSerializer.Deserialize<AppointmentDTO>(responseContent, 
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (result != null)
                        {
                            ViewBag.Message = "Appointment booked successfully!";
                            _logger.LogInformation("Appointment created with ID: {AppointmentId}", result.AppointmentId);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        // Try to parse error response
                        try
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("API error response: {StatusCode} - {Content}", response.StatusCode, errorContent);
                            
                            using (var doc = System.Text.Json.JsonDocument.Parse(errorContent))
                            {
                                if (doc.RootElement.TryGetProperty("message", out var messageElement))
                                {
                                    ViewBag.Error = messageElement.GetString() ?? "Failed to book appointment";
                                }
                                else
                                {
                                    ViewBag.Error = $"API Error: {response.StatusCode} - {errorContent}";
                                }
                            }
                        }
                        catch
                        {
                            var errorText = await response.Content.ReadAsStringAsync();
                            ViewBag.Error = $"API returned error: {response.StatusCode} - {errorText}";
                            _logger.LogError("Failed to parse error response: {StatusCode} - {Content}", response.StatusCode, errorText);
                        }
                    }
                }

                return View("Book", model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                _logger.LogError(ex, "Error booking appointment: {Error}", ex.Message);
                return View("Book", model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                    _apiClient.SetAuthToken(token);

                var appointment = await _apiClient.GetAsync<AppointmentDTO>($"appointments/{id}");
                if (appointment == null)
                {
                    return NotFound();
                }
                return View(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment {Id}", id);
                return NotFound();
            }
        }
    }
}
