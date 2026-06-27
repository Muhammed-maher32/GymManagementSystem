using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Contracts
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct);

        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct);
        Task<MemberDetailsViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct);
        Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct);
        Task<bool> RemoveMemberAsync(int id, CancellationToken ct);
    }
}
