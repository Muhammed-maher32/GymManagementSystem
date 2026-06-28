using GymSystem.DAL.Data.AppDbContexts;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repositories.Interfaces;

namespace GymSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork //UoW
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = [];
        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.TryGetValue(typeName, out var repository))
                return (IGenericRepository<TEntity>)repository;

            var repo = new GenericRepository<TEntity>(_dbContext);
            //_repositories.Add(typeName, repo);
            _repositories[typeName] = repo;
            return repo;
        }


        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
