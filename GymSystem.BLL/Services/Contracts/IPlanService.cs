using GymSystem.BLL.Services.Common;
using GymSystem.BLL.ViewModels.PlanViewModels;

namespace GymSystem.BLL.Services.Contracts
{
    public interface IPlanService
    {
        Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct);
        Task<Result<PlanViewModel?>> GetPlanByIdAsync(int id, CancellationToken ct);
    }
}
