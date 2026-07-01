using GymSystem.DAL.Models;
using System.Linq.Expressions;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);

    }
}
