using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class BillsController : Controller
    {
        private readonly ApiService _apiService;

        public BillsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _apiService.GetAsync<List<BillDto>>("bills");
            return View(bills);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateAppointmentsAsync();
            ViewBag.StatusOptions = new[]
            {
                new SelectListItem("Paid", "Paid"),
                new SelectListItem("Unpaid", "Unpaid")
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBillDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAppointmentsAsync();
                ViewBag.StatusOptions = new[]
                {
                    new SelectListItem("Paid", "Paid"),
                    new SelectListItem("Unpaid", "Unpaid")
                };
                return View(model);
            }

            await _apiService.PostAsync<BillDto>("bills", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _apiService.GetAsync<BillDto>($"bills/{id}");
            if (bill == null)
                return NotFound();

            await PopulateAppointmentsAsync();
            ViewBag.StatusOptions = new[]
            {
                new SelectListItem("Paid", "Paid"),
                new SelectListItem("Unpaid", "Unpaid")
            };

            var updateModel = new UpdateBillDto
            {
                AppointmentId = bill.AppointmentId,
                ConsultationFee = bill.ConsultationFee,
                MedicineCharges = bill.MedicineCharges,
                PaymentStatus = bill.PaymentStatus
            };

            ViewBag.BillId = id;
            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateBillDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAppointmentsAsync();
                ViewBag.StatusOptions = new[]
                {
                    new SelectListItem("Paid", "Paid"),
                    new SelectListItem("Unpaid", "Unpaid")
                };
                return View(model);
            }

            await _apiService.PutAsync($"bills/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _apiService.GetAsync<BillDto>($"bills/{id}");
            if (bill == null)
                return NotFound();

            return View(bill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync($"bills/{id}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBill(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var bill = await _apiService.GetAsync<BillDto>($"bills/{id}");
                if (bill == null)
                {
                    TempData["ErrorMessage"] = "Bill not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Update bill status to Paid
                var updateModel = new UpdateBillDto
                {
                    AppointmentId = bill.AppointmentId,
                    ConsultationFee = bill.ConsultationFee,
                    MedicineCharges = bill.MedicineCharges,
                    PaymentStatus = "Paid"
                };

                await _apiService.PutAsync($"bills/{id}", updateModel);
                TempData["SuccessMessage"] = $"Bill #{id} has been paid successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to process payment: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var bill = await _apiService.GetAsync<BillDto>($"bills/{id}");
            if (bill == null)
                return NotFound();

            return View(bill);
        }

        // GET: /Bills/Statements - Patient billing statements with filtering
        public async Task<IActionResult> Statements(string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get bills for the current patient
            var bills = await _apiService.GetAsync<List<BillDto>>($"bills/patient/{userId.Value}") ?? new List<BillDto>();

            // Apply filters
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                bills = bills.Where(b => b.PaymentStatus == status).ToList();
            }

            if (fromDate.HasValue)
            {
                bills = bills.Where(b => b.AppointmentDate >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                bills = bills.Where(b => b.AppointmentDate <= toDate.Value).ToList();
            }

            // Calculate summary statistics
            var totalAmount = bills.Sum(b => b.TotalAmount);
            var paidAmount = bills.Where(b => b.PaymentStatus == "Paid").Sum(b => b.TotalAmount);
            var unpaidAmount = bills.Where(b => b.PaymentStatus == "Unpaid").Sum(b => b.TotalAmount);

            ViewBag.TotalAmount = totalAmount;
            ViewBag.PaidAmount = paidAmount;
            ViewBag.UnpaidAmount = unpaidAmount;
            ViewBag.TotalBills = bills.Count;
            ViewBag.PaidBills = bills.Count(b => b.PaymentStatus == "Paid");
            ViewBag.UnpaidBills = bills.Count(b => b.PaymentStatus == "Unpaid");

            // Preserve filter values for the view
            ViewBag.SelectedStatus = status ?? "All";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            // Order by date descending (most recent first)
            return View(bills.OrderByDescending(b => b.AppointmentDate).ToList());
        }

        // GET: /Bills/StatementPdf/{id} - Download individual bill as statement
        public async Task<IActionResult> StatementPdf(int id)
        {
            var bill = await _apiService.GetAsync<BillDto>($"bills/{id}");
            if (bill == null)
                return NotFound();

            // For now, return the receipt view which is print-friendly
            return View("Receipt", bill);
        }

        private async Task PopulateAppointmentsAsync()
        {
            var appointments = await _apiService.GetAsync<List<AppointmentDto>>("appointments");
            ViewBag.Appointments = (appointments ?? new List<AppointmentDto>()).Select(a => new SelectListItem($"#{a.AppointmentId} - {a.PatientName}", a.AppointmentId.ToString())).ToList();
        }
    }
}
