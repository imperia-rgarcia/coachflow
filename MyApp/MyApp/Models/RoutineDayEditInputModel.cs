using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class RoutineDayEditInputModel
    {
        public RoutineDayEditInputModel()
        {
            DayName = string.Empty;
            Exercises = new List<RoutineDayExerciseInputModel>();
        }

        [Display(Name = "Día del microciclo")]
        public string DayName { get; set; }

        public List<RoutineDayExerciseInputModel> Exercises { get; set; }

        public void EnsureExerciseEntry()
        {
            if (Exercises.Count == 0)
            {
                RoutineDayExerciseInputModel exercise = new RoutineDayExerciseInputModel();
                Exercises.Add(exercise);
            }
        }
    }
}
