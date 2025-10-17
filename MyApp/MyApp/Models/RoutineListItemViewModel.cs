using System;

namespace MyApp.Models
{
    public class RoutineListItemViewModel
    {
        public int RoutineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PhasesSummary { get; set; } = string.Empty;

        public string MicrocycleDaysSummary { get; set; } = string.Empty;

        public int TotalMicrocycles { get; set; }

        public int PhaseCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
