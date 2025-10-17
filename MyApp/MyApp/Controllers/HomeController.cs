using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoutineCreator()
        {
            RoutineInputModel viewModel = new RoutineInputModel();

            EnsureMinimumEntries(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoutineCreator(RoutineInputModel inputModel)
        {
            EnsureMinimumEntries(inputModel);

            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            string phasesJson = JsonSerializer.Serialize(inputModel.Phases);
            List<RoutineDayEditInputModel> dayDetails = BuildMicrocycleDayDetailsFromNames(inputModel.MicrocycleDays);
            string microcycleDaysJson = JsonSerializer.Serialize(dayDetails);
            int totalMicrocycles = inputModel.GetTotalMicrocycles();

            Routine routine = new Routine
            {
                Name = inputModel.Name,
                PhasesJson = phasesJson,
                MicrocycleDaysJson = microcycleDaysJson,
                TotalMicrocycles = totalMicrocycles,
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Routines.Add(routine);
            await _dbContext.SaveChangesAsync();

            TempData["RoutineSaved"] = "Rutina guardada correctamente.";

            return RedirectToAction(nameof(RoutineCreator));
        }

        [HttpGet]
        public async Task<IActionResult> RoutineList()
        {
            List<Routine> routines = await _dbContext.Routines
                .OrderByDescending(routine => routine.CreatedAtUtc)
                .ToListAsync();

            List<RoutineListItemViewModel> viewModel = new List<RoutineListItemViewModel>();

            foreach (Routine routine in routines)
            {
                List<RoutinePhaseInputModel> defaultPhaseList = new List<RoutinePhaseInputModel>();
                List<RoutinePhaseInputModel>? phases = JsonSerializer.Deserialize<List<RoutinePhaseInputModel>>(routine.PhasesJson);
                List<RoutinePhaseInputModel> phaseList = phases ?? defaultPhaseList;

                List<RoutineDayEditInputModel> microcycleDayDetails = DeserializeMicrocycleDayDetails(routine.MicrocycleDaysJson);
                List<string> microcycleDaysList = new List<string>();

                foreach (RoutineDayEditInputModel dayDetail in microcycleDayDetails)
                {
                    if (!string.IsNullOrWhiteSpace(dayDetail.DayName))
                    {
                        microcycleDaysList.Add(dayDetail.DayName);
                    }
                }

                List<string> phaseSummaries = new List<string>();

                foreach (RoutinePhaseInputModel phase in phaseList)
                {
                    string summary = string.Format("{0} ({1})", phase.Name, phase.Microcycles);
                    phaseSummaries.Add(summary);
                }

                string phasesSummary = string.Join(", ", phaseSummaries);

                if (string.IsNullOrEmpty(phasesSummary))
                {
                    phasesSummary = "Sin fases registradas";
                }

                string microcycleDaysSummary = string.Join(", ", microcycleDaysList);

                if (string.IsNullOrEmpty(microcycleDaysSummary))
                {
                    microcycleDaysSummary = "Sin días registrados";
                }

                RoutineListItemViewModel listItem = new RoutineListItemViewModel
                {
                    RoutineId = routine.RoutineId,
                    Name = routine.Name,
                    PhasesSummary = phasesSummary,
                    MicrocycleDaysSummary = microcycleDaysSummary,
                    TotalMicrocycles = routine.TotalMicrocycles,
                    PhaseCount = phaseList.Count,
                    CreatedAtUtc = routine.CreatedAtUtc
                };

                viewModel.Add(listItem);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RoutineEdit(int id)
        {
            Routine? routine = await _dbContext.Routines
                .FirstOrDefaultAsync(routineItem => routineItem.RoutineId == id);

            if (routine == null)
            {
                return NotFound();
            }

            List<RoutinePhaseInputModel>? phases = JsonSerializer.Deserialize<List<RoutinePhaseInputModel>>(routine.PhasesJson);
            List<RoutinePhaseInputModel> phaseList = phases ?? new List<RoutinePhaseInputModel>();

            List<RoutineDayEditInputModel> microcycleDayDetails = DeserializeMicrocycleDayDetails(routine.MicrocycleDaysJson);

            RoutineEditViewModel viewModel = new RoutineEditViewModel
            {
                RoutineId = routine.RoutineId,
                Name = routine.Name,
                Phases = phaseList,
                Days = microcycleDayDetails
            };

            foreach (RoutineDayEditInputModel day in viewModel.Days)
            {
                day.EnsureExerciseEntry();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoutineEdit(RoutineEditViewModel inputModel)
        {
            Routine? routine = await _dbContext.Routines
                .FirstOrDefaultAsync(routineItem => routineItem.RoutineId == inputModel.RoutineId);

            if (routine == null)
            {
                return NotFound();
            }

            List<RoutinePhaseInputModel>? phases = JsonSerializer.Deserialize<List<RoutinePhaseInputModel>>(routine.PhasesJson);
            inputModel.Phases = phases ?? new List<RoutinePhaseInputModel>();
            inputModel.Name = routine.Name;

            if (inputModel.Days == null)
            {
                inputModel.Days = new List<RoutineDayEditInputModel>();
            }

            if (!ModelState.IsValid)
            {
                foreach (RoutineDayEditInputModel day in inputModel.Days)
                {
                    day.EnsureExerciseEntry();
                }

                return View(inputModel);
            }

            List<RoutineDayEditInputModel> sanitizedDays = SanitizeRoutineDayEntries(inputModel.Days);
            string microcycleDaysJson = JsonSerializer.Serialize(sanitizedDays);

            routine.MicrocycleDaysJson = microcycleDaysJson;

            await _dbContext.SaveChangesAsync();

            TempData["RoutineUpdated"] = "Rutina actualizada correctamente.";

            return RedirectToAction(nameof(RoutineEdit), new { id = routine.RoutineId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static void EnsureMinimumEntries(RoutineInputModel model)
        {
            if (model.Phases.Count == 0)
            {
                model.Phases.Add(new RoutinePhaseInputModel());
            }

            if (model.MicrocycleDays.Count == 0)
            {
                model.MicrocycleDays.Add(string.Empty);
            }
        }

        private static List<RoutineDayEditInputModel> BuildMicrocycleDayDetailsFromNames(List<string> dayNames)
        {
            List<RoutineDayEditInputModel> dayDetails = new List<RoutineDayEditInputModel>();

            foreach (string dayName in dayNames)
            {
                RoutineDayEditInputModel dayDetail = new RoutineDayEditInputModel
                {
                    DayName = dayName
                };

                dayDetail.EnsureExerciseEntry();
                dayDetails.Add(dayDetail);
            }

            return dayDetails;
        }

        private static List<RoutineDayEditInputModel> DeserializeMicrocycleDayDetails(string microcycleDaysJson)
        {
            List<RoutineDayEditInputModel> dayDetails = new List<RoutineDayEditInputModel>();

            if (string.IsNullOrWhiteSpace(microcycleDaysJson))
            {
                return dayDetails;
            }

            try
            {
                List<RoutineDayEditInputModel>? detailedDays = JsonSerializer.Deserialize<List<RoutineDayEditInputModel>>(microcycleDaysJson);

                if (detailedDays != null)
                {
                    foreach (RoutineDayEditInputModel detailedDay in detailedDays)
                    {
                        detailedDay.EnsureExerciseEntry();
                        dayDetails.Add(detailedDay);
                    }

                    return dayDetails;
                }
            }
            catch (JsonException)
            {
            }

            try
            {
                List<string>? dayNames = JsonSerializer.Deserialize<List<string>>(microcycleDaysJson);

                if (dayNames != null)
                {
                    dayDetails = BuildMicrocycleDayDetailsFromNames(dayNames);
                    return dayDetails;
                }
            }
            catch (JsonException)
            {
            }

            return dayDetails;
        }

        private static List<RoutineDayEditInputModel> SanitizeRoutineDayEntries(List<RoutineDayEditInputModel> days)
        {
            List<RoutineDayEditInputModel> sanitizedDays = new List<RoutineDayEditInputModel>();

            foreach (RoutineDayEditInputModel day in days)
            {
                RoutineDayEditInputModel sanitizedDay = new RoutineDayEditInputModel
                {
                    DayName = day.DayName
                };

                if (day.Exercises == null)
                {
                    day.Exercises = new List<RoutineDayExerciseInputModel>();
                }

                foreach (RoutineDayExerciseInputModel exercise in day.Exercises)
                {
                    bool hasContent = !string.IsNullOrWhiteSpace(exercise.Exercise)
                        || !string.IsNullOrWhiteSpace(exercise.Comment)
                        || !string.IsNullOrWhiteSpace(exercise.Series)
                        || !string.IsNullOrWhiteSpace(exercise.Repetitions)
                        || !string.IsNullOrWhiteSpace(exercise.Rir);

                    if (hasContent)
                    {
                        RoutineDayExerciseInputModel sanitizedExercise = new RoutineDayExerciseInputModel
                        {
                            Exercise = exercise.Exercise,
                            Comment = exercise.Comment,
                            Series = exercise.Series,
                            Repetitions = exercise.Repetitions,
                            Rir = exercise.Rir
                        };

                        sanitizedDay.Exercises.Add(sanitizedExercise);
                    }
                }

                sanitizedDay.EnsureExerciseEntry();
                sanitizedDays.Add(sanitizedDay);
            }

            return sanitizedDays;
        }
    }
}
