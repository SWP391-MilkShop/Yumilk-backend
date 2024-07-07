namespace NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Save changes to the database in a single transaction
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();

    void Detach<TEntity>(TEntity entity) where TEntity : class;
}