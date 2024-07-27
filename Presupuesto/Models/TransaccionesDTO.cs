 using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Presupuesto.Models
{
    public class TransaccionesDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Debe agregar una cantidad")]
        [Display(Name = "Cantidad")]
        public int CantidadDepositada { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Código de la transacción del boucher o comprobante bancario")]
        public string Codigo_transaccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Fecha de la transacción")]
        public DateTime FechaTransaccion { get; set; }

        [Display(Name = "Imagen de la transacción")]
        [Required(ErrorMessage = "Debe agregar una imagen")]
        //[DataType(DataType.Upload)]
        //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".pdf" })]
        public IFormFile File { get; set; } 
        public int? IdPersona { get; set; } 

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
                    return new ValidationResult($"Only files with extensions {string.Join(", ", _extensions)} are allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
