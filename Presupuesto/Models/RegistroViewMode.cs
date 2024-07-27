using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class RegistroViewMode
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre de usuario")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas deben coincidir")]
        public string ConfirmPassword { get; set; }
    }
}
