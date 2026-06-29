using GymSystem.BLL.Services.Common;
using GymSystem.BLL.ViewModels.MemberViewModels;

namespace GymSystem.BLL.Services.Contracts
{
    public interface IMemberService
    {
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct);

        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct);
        Task<Result<MemberDetailsViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken ct);
        Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int memberId, CancellationToken ct);
        Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int memberId, CancellationToken ct);
        Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct);
        Task<Result> RemoveMemberAsync(int id, CancellationToken ct);
    }
}
