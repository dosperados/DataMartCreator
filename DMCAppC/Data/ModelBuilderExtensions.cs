using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;


namespace DMCApp.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var deletedProperty = Expression.Property(parameter, nameof(BaseEntity.Deleted));
                    var zero = Expression.Constant((byte)0, typeof(byte));
                    var condition = Expression.Equal(deletedProperty, zero);
                    var lambda = Expression.Lambda(condition, parameter);

                    //entityType.SetQueryFilter(lambda); //нужна ли эта строчка?
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    
                }
            }
        }
    }
}
