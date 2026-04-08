using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Web.Services;

namespace SmartHealthcare.Web.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly IApiClient _apiClient;

        public PrescriptionsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
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

                _apiClient.SetAuthToken(token);

                // For now, showing demo prescriptions
                // In future, fetch from API: var prescriptions = await _apiClient.GetAsync<dynamic>("prescriptions");
                
                var dummyPrescriptions = new List<dynamic>
                {
                    new { id = 1, patientName = "You", doctorName = "Dr. Sarah Johnson", medicineName = "Aspirin 500mg", dosage = "1 tablet twice daily", startDate = "2026-04-01", endDate = "2026-04-30", status = "Active" },
                    new { id = 2, patientName = "You", doctorName = "Dr. James Wilson", medicineName = "Vitamin D 1000IU", dosage = "1 capsule daily", startDate = "2026-03-15", endDate = "2026-06-15", status = "Active" }
                };

                return View(dummyPrescriptions);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load prescriptions: " + ex.Message;
                return View(new List<dynamic>());
            }
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

                // For now, using dummy prescriptions - in future, fetch from API
                var allPrescriptions = new List<dynamic>
                {
                    new { id = "1", patientName = "You", doctorName = "Dr. Sarah Johnson", medicineName = "Aspirin 500mg", dosage = "1 tablet twice daily", startDate = "2026-04-01", endDate = "2026-04-30", status = "Active", instructions = "Take with food. Do not exceed 4 tablets per day.", sideEffects = "May cause mild stomach irritation", precautions = "Avoid if allergic to aspirin. Consult doctor if pregnant." },
                    new { id = "2", patientName = "You", doctorName = "Dr. James Wilson", medicineName = "Vitamin D 1000IU", dosage = "1 capsule daily", startDate = "2026-03-15", endDate = "2026-06-15", status = "Active", instructions = "Take once daily with breakfast.", sideEffects = "Generally well tolerated", precautions = "Store in cool, dry place." }
                };

                var prescription = allPrescriptions.FirstOrDefault(p => p.id == id);
                
                if (prescription == null)
                {
                    TempData["Error"] = "Prescription not found";
                    return RedirectToAction("Index");
                }

                return View(prescription);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading prescription: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
