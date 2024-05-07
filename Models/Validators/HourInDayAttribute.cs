using System.ComponentModel.DataAnnotations;

namespace Models.Validators
{
    public class HourInDayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var timeValue = value as TimeSpan?;

            if (timeValue != null)
            {
                // Check if the time value is between 00:00 and 23:59
                if (timeValue.Value >= TimeSpan.Zero && timeValue.Value < TimeSpan.FromDays(1))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage ?? "The input value should represent a valid hour in a day (00:00 to 23:59).");
        }
    }
}
