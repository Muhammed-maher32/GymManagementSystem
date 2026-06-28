using GymSystem.DAL.Models;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        Task<int> SaveChangesAsync(CancellationToken ct);
        ISessionRepository SessionRepository { get; }
    }
}
