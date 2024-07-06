namespace NET1814_MilkShop.Repositories.Data.Interfaces;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }

    DateTime? ModifiedAt { get; set; }

    DateTime? DeletedAt { get; set; }
}