using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class RoutineDayEditInputModel
    {
        public RoutineDayEditInputModel()
        {
            DayName = string.Empty;
            Exercise = string.Empty;
            Comment = string.Empty;
            Series = string.Empty;
            Repetitions = string.Empty;
            Rir = string.Empty;
        }

        [Display(Name = "Día del microciclo")]
        public string DayName { get; set; }

        [Display(Name = "Ejercicio")]
        public string Exercise { get; set; }

        [Display(Name = "Comentario")]
        public string Comment { get; set; }

        [Display(Name = "Series")]
        public string Series { get; set; }

        [Display(Name = "Repeticiones")]
        public string Repetitions { get; set; }

        [Display(Name = "RIR")]
        public string Rir { get; set; }
    }
}
