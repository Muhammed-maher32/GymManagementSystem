using GymSystem.DAL.Data.AppDbContexts;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;
        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }


        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _set.AnyAsync(predicate, ct);
        }

        public void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _set.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct)
        {
            return await _set.ToListAsync(ct);
        }

        public async Task<TEntity> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await _set.FindAsync(id, ct);
            return entity;
        }

        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }
    }
}
