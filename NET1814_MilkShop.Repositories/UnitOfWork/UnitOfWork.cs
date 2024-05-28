using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
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
        try
        {
            UpdateAuditableEntities();
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving changes", ex);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private void UpdateAuditableEntities()
    {
        var entries =
            _context.ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
                entityEntry.Property(a => a.CreatedAt).CurrentValue = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Modified)
                entityEntry.Property(a => a.ModifiedAt).CurrentValue = DateTime.UtcNow;
        }
    }
}