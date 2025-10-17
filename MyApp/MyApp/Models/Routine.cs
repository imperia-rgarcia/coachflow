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
        [MaxLength(100)]
        public string PrimaryObjective { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(10, 180)]
        public int SessionDurationMinutes { get; set; }

        [Range(1, 7)]
        public int WeeklyFrequency { get; set; }

        [Required]
        [MaxLength(50)]
        public string EnergyLevel { get; set; } = string.Empty;

        [Required]
        public string PreferredDays { get; set; } = string.Empty;

        public bool IsDraft { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
