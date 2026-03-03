using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class DbConfig : IEntityTypeConfiguration<Entities.Db>
    {
        public void Configure(EntityTypeBuilder<Entities.Db> builder)
        {
            builder.ToTable("DB", "Setting");

            builder.HasKey(e => e.DBId)
                   .HasName("PK_Setting_DB_DBId");

            builder.Property(e => e.DBId)
                   .HasColumnName("DBId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.DBName)
                   .HasColumnName("DBName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(2500);

            builder.Property(e => e.Connection)
                   .HasMaxLength(255);

            builder.HasIndex(e => e.DBName)
                   .IsUnique()
                   .HasDatabaseName("UQ_Setting_DB_DBName_IndexUniqueConstraint");

            builder.ConfigureBaseEntity();
        }
    }
}
