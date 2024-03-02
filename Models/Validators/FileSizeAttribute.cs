using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Models.Validators
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        private readonly int _minFileSize;

        public FileSizeAttribute(int maxFileSize = 2 * 1024 * 1024, int minFileSize = 0)
        {
            _maxFileSize = maxFileSize;
            _minFileSize = minFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size cannot exceed {_maxFileSize / 1024} KB.");
                }

                if (file.Length < _minFileSize)
                {
                    return new ValidationResult($"File size must be at least {_minFileSize / 1024} KB.");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid file.");
        }
    }
}
