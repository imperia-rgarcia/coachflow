using System.Collections.Generic;

namespace MyApp.Models
{
    public class RoutineEditViewModel
    {
        public RoutineEditViewModel()
        {
            Phases = new List<RoutinePhaseInputModel>();
            Days = new List<RoutineDayEditInputModel>();
        }

        public int RoutineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<RoutinePhaseInputModel> Phases { get; set; }

        public List<RoutineDayEditInputModel> Days { get; set; }
    }
}
