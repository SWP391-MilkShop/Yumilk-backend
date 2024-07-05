using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Save changes to the database in a single transaction
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
        void Detach<TEntity>(TEntity entity) where TEntity : class;
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            int result;

            // Wrap the entire save process in a transaction
            await using var dbContextTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                UpdateAuditableEntities();
                result = await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }
            catch (Exception)
            {
                //Log Exception Handling message
                result = -1;
                await dbContextTransaction.RollbackAsync();
            }

            return result;
            //try
            //{
            //    UpdateAuditableEntities();
            //    return await _context.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("An error occurred while saving changes", ex);
            //}
        }

        private void UpdateAuditableEntities()
        {
            var entries =
                _context.ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(a => a.CreatedAt).CurrentValue = DateTime.UtcNow;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(a => a.ModifiedAt).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}
