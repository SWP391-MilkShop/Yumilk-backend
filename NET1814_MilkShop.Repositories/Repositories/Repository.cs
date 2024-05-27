using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public abstract class Repository<TEntity>
        where TEntity : class, IAuditableEntity
    {
        protected readonly AppDbContext _context;
        protected readonly IQueryable<TEntity> _query;

        protected Repository(AppDbContext context)
        {
            _context = context;
            _query = _context.Set<TEntity>().Where(x => x.DeletedAt == null).AsNoTracking();
        }

        public virtual void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual async Task<TEntity?> GetById(Guid id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null && entity.DeletedAt == null)
            {
                return entity;
            }
            return null;
        }

        public virtual async Task<TEntity?> GetById(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null && entity.DeletedAt == null)
            {
                return entity;
            }
            return null;
        }
    }
}
