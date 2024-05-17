using NET1814_MilkShop.Repositories.Data;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public abstract class Repository<TEntity>
        where TEntity : class
    {
        protected readonly AppDbContext _context;

        protected Repository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }
}
