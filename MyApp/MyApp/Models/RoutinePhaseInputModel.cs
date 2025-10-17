using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class RoutinePhaseInputModel
    {
        [Required(ErrorMessage = "El nombre de la fase es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de la fase no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre de la fase")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 52, ErrorMessage = "Los microciclos deben estar entre 1 y 52.")]
        [Display(Name = "Microciclos")]
        public int Microcycles { get; set; }
    }
}
