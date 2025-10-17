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
            string microcycleDaysJson = JsonSerializer.Serialize(inputModel.MicrocycleDays);
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

                List<string> defaultMicrocycleDaysList = new List<string>();
                List<string>? microcycleDays = JsonSerializer.Deserialize<List<string>>(routine.MicrocycleDaysJson);
                List<string> microcycleDaysList = microcycleDays ?? defaultMicrocycleDaysList;

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

            List<string>? microcycleDays = JsonSerializer.Deserialize<List<string>>(routine.MicrocycleDaysJson);
            List<string> microcycleDaysList = microcycleDays ?? new List<string>();

            RoutineEditViewModel viewModel = new RoutineEditViewModel
            {
                RoutineId = routine.RoutineId,
                Name = routine.Name,
                Phases = phaseList
            };

            foreach (string day in microcycleDaysList)
            {
                RoutineDayEditInputModel dayModel = new RoutineDayEditInputModel
                {
                    DayName = day
                };

                viewModel.Days.Add(dayModel);
            }

            return View(viewModel);
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
    }
}
