using Microsoft.EntityFrameworkCore;

namespace WorldBank.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T>, IUnitOfWork where T : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        public UnitOfWork(T context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(Context);
            return (Repository<TEntity>)_repositories[type];
        }

        public T Context { get; }

        public int Commit()
        {
            return 1;
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
    #region Interfaces
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        int SaveChanges();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
    #endregion
}
