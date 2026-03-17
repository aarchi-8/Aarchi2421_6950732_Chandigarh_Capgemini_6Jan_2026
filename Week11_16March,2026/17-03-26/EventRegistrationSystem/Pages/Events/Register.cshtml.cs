using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EventRegistrationSystem.Models;

namespace EventRegistrationSystem.Pages.Events   
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public EventRegistration Registration { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            Registration.Id = EventData.Registrations.Count + 1;
            EventData.Registrations.Add(Registration);

            return RedirectToPage("Index");
        }
    }
}