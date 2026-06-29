using GymSystem.BLL.Services.Common;
using GymSystem.BLL.ViewModels.SessionViewModels;

namespace GymSystem.BLL.Services.Contracts
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel?>> GetAllSessionsAsync(CancellationToken ct);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);

    }
}
