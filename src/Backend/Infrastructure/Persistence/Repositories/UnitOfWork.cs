using Application.Interfaces.Repositories;
using Domain.Common;
using Persistence.Context;
using System.Collections;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private Hashtable _repositories;
        private bool disposed;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private Hashtable Repositories
        {
            get
            {
                if (_repositories == null)
                    _repositories = new Hashtable();
                return _repositories;
            }
        }

        public IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity
        {
            var type = typeof(T).Name;

            if (!Repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);
                Repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)Repositories[type];
        }

        public void SaveChanges()
        {
            Console.WriteLine("save işlemi için");
            //_dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("save changes işlemi için");
            //await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Rollback()
        {
            //_dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        public Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //_dbContext.Dispose();
                _repositories?.Clear();
            }

            disposed = true;
        }
    }
}
