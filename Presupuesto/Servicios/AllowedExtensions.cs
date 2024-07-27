using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Servicios
{
    public interface AllowedExtensions
    {
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(fileExtension))
                {
                    return new ValidationResult($"Solo archivos tipo {string.Join(", ", _extensions)} son permitidos.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
