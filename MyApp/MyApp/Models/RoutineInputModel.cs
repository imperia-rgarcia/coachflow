using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class RoutineInputModel
    {
        public RoutineInputModel()
        {
            Phases = new List<RoutinePhaseInputModel>();
            MicrocycleDays = new List<string>();
        }

        [Required(ErrorMessage = "El nombre de la rutina es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de la rutina no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre de la rutina")]
        public string Name { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Debes agregar al menos una fase.")]
        [Display(Name = "Fases de la rutina")]
        public List<RoutinePhaseInputModel> Phases { get; set; }

        [MinLength(1, ErrorMessage = "Debes indicar al menos un día para el microciclo.")]
        [Display(Name = "Días del microciclo")]
        public List<string> MicrocycleDays { get; set; }

        public int GetTotalMicrocycles()
        {
            int totalMicrocycles = 0;

            foreach (RoutinePhaseInputModel phase in Phases)
            {
                totalMicrocycles += phase.Microcycles;
            }

            return totalMicrocycles;
        }
    }
}
