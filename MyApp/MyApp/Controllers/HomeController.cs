using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoutineCreator(RoutineInputModel inputModel, string? saveAction)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            List<string> selectedDays = inputModel.PreferredDays ?? new List<string>();

            Routine routine = new Routine
            {
                Name = inputModel.Name,
                PrimaryObjective = inputModel.PrimaryObjective,
                Description = inputModel.Description,
                SessionDurationMinutes = inputModel.SessionDurationMinutes,
                WeeklyFrequency = inputModel.WeeklyFrequency,
                EnergyLevel = inputModel.EnergyLevel,
                PreferredDays = string.Join(",", selectedDays),
                IsDraft = string.Equals(saveAction, "Draft", StringComparison.OrdinalIgnoreCase),
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Routines.Add(routine);
            await _dbContext.SaveChangesAsync();

            TempData["RoutineSaved"] = routine.IsDraft ? "Rutina guardada como borrador." : "Rutina generada y guardada.";

            return RedirectToAction(nameof(RoutineCreator));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
