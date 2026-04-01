using System.ComponentModel.DataAnnotations;

namespace EventBooking.API.Validation;

public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
    {
        ErrorMessage = "Event date must be a future date.";
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.UtcNow;
        }
        return false;
    }
}
