using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymSystemG04.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }
        #region CreateSession

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownListsAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListsAsync(ct);
                return View(model);
            }
            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "SessionCreatedSuccessfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropDownListsAsync(ct);
            return View(model);
        }

        private async Task PopulateDropDownListsAsync(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(ct), "Id", "CategoryName");
        }
        #endregion
    }
}
