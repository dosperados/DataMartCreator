using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    public static class BaseEntityConfig
    {
        public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : BaseEntity
        {
            builder.Property(e => e.ModifiedAt)
                   .ValueGeneratedOnAddOrUpdate()
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            builder.Property(e => e.ModifiedBy)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.Deleted)
                   .HasColumnType("tinyint")
                   .HasDefaultValue(0)
                   .IsRequired();
        }
    }
}
