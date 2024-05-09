using System.ComponentModel.DataAnnotations;

namespace Models.Validators
{
    /// <summary>
    /// Date range validator
    /// </summary>
    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate;
        private readonly DateTime _maxDate;
        /// <summary>
        /// Validate a date by min and max values.
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        public DateRangeAttribute(string minDate, string maxDate)
        {
            _minDate = DateTime.Parse(minDate);
            _maxDate = DateTime.Parse(maxDate);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date <= _maxDate && date >= _minDate)
                    return ValidationResult.Success;

            }
            return new ValidationResult($"Date should be beteen '{_minDate.ToLongDateString()}' and '{_maxDate.ToLongDateString()}'");
        }
    }
}
