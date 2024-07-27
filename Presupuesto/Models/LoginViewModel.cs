using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre de usuario")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
         
        [Display(Name = "Recuérdame")]
        public bool RememberMe { get; set; }
    }
}
