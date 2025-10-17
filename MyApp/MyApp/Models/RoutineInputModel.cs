using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class RoutineInputModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre de la rutina")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El objetivo es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El objetivo no puede exceder 100 caracteres.")]
        [Display(Name = "Objetivo principal")]
        public string PrimaryObjective { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Range(10, 180, ErrorMessage = "La duración debe estar entre 10 y 180 minutos.")]
        [Display(Name = "Duración por sesión")]
        public int SessionDurationMinutes { get; set; }

        [Range(1, 7, ErrorMessage = "La frecuencia debe estar entre 1 y 7 sesiones.")]
        [Display(Name = "Frecuencia semanal")]
        public int WeeklyFrequency { get; set; }

        [Required(ErrorMessage = "Selecciona el nivel de energía.")]
        [MaxLength(50)]
        [Display(Name = "Nivel de energía esperado")]
        public string EnergyLevel { get; set; } = string.Empty;

        [Display(Name = "Días preferidos")]
        public List<string> PreferredDays { get; set; } = new List<string>();

        public bool IsDraft { get; set; }
    }
}
