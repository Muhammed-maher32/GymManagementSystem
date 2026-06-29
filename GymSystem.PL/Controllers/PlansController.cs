using GymSystem.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GymSystemG04.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);

            return View(plans.Value);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanByIdAsync(id, ct);

            if (!plan.Success)
            {
                TempData["ErrorMessage"] = plan.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(plan.Value);
        }
    }
}
