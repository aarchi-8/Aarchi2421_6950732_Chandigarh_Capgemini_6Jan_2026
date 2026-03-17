using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    public List<EventRegistration> Registrations { get; set; }

    public void OnGet()
    {
        Registrations = EventData.Registrations;
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = EventData.Registrations.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            EventData.Registrations.Remove(item);
        }

        return RedirectToPage();
    }
}