using AutoMapper;
using GymSystem.BLL.Services.Common;
using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repositories.Interfaces;

namespace GymSystem.BLL.Services
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct);
            var vms = _mapper.Map<IEnumerable<PlanViewModel>>(plans);
            return Result<IEnumerable<PlanViewModel>>.Ok(vms);
        }

        public async Task<Result<PlanViewModel?>> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result<PlanViewModel>.NotFound("Plan not found.")!;

            var vm = _mapper.Map<PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(vm)!;
        }
    }
}
