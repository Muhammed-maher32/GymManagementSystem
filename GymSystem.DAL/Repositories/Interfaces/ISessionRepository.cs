using GymSystem.DAL.Models;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct);
        Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken ct);
        Task<Dictionary<int, int>> GetBookedSlotsCountsAsync(IEnumerable<int> sessionIds, CancellationToken ct);
    }
}
