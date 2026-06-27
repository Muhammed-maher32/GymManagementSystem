using GymSystem.BLL.Services.Common;
using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        public IActionResult Create(CancellationToken ct)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "SessionCreatedSuccessfully";
                RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            return View(model);
        }


        #endregion
    }
}
