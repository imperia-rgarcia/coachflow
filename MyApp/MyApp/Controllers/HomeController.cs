using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
