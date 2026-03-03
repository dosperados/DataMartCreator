using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class ColumnConfig : IEntityTypeConfiguration<Entities.Column>
    {
        public void Configure(EntityTypeBuilder<Entities.Column> builder)
        {
            builder.ToTable("Column", "Setting");

            builder.HasKey(e => e.ColumnId)
                   .HasName("PK_Setting_Column_ColumnId");

            builder.Property(e => e.ColumnId)
                   .HasColumnName("ColumnId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.TableId)
                   .HasColumnName("TableId")
                   .IsRequired();

            builder.Property(e => e.VersionId)
                   .HasColumnName("VersionId")
                   .IsRequired();

            builder.Property(e => e.ColumnName)
                   .HasColumnName("ColumnName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.DataType)
                   .HasColumnName("DataType")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.IsNullable)
                   .HasColumnType("tinyint")
                   .HasDefaultValue(1)
                   .IsRequired();

            builder.Property(e => e.IsPrimaryKey)
                   .HasColumnType("tinyint")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.CollationName)
                   .HasMaxLength(255);

            builder.Property(e => e.Description)
                   .HasMaxLength(2500);

            builder.HasIndex(e => new { e.TableId, e.ColumnName })
                   .IsUnique()
                   .HasDatabaseName("UQ_Setting_Column_TableId_ColumnName_UniqueConstraint");

            builder.HasOne(d => d.Table)
                   .WithMany(p => p.Columns)
                   .HasForeignKey(d => d.TableId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Setting_Column_Setting_Table");

            builder.ConfigureBaseEntity();
        }
    }
}
