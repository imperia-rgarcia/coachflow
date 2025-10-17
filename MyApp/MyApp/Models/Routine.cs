using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class Routine
    {
        public int RoutineId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhasesJson { get; set; } = string.Empty;

        [Required]
        public string MicrocycleDaysJson { get; set; } = string.Empty;

        public int TotalMicrocycles { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
