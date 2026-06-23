using GymSystem.DAL.AppDbContexts;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymSystemG04.Controllers
{
    public class PlansController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepo;

        public PlansController(IGenericRepository<Plan> planRepository)
        {
            _planRepo = planRepository;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepo.GetAllAsync(ct);

            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);

            if(plan == null)
                return RedirectToAction(nameof(Index));

            return View(plan);
        }
    }
}
