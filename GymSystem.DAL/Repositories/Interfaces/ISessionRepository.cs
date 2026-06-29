using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct);
        Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken ct);
        Task<Dictionary<int, int>> GetBookedSlotsCountsAsync(IEnumerable<int> sessionIds, CancellationToken ct);
    }
}
