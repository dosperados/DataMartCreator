using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class ColumnConfig : IEntityTypeConfiguration<Column>
    {
        public void Configure(EntityTypeBuilder<Column> builder)
        {
            builder.ToTable("Column", "Setting");

            builder.HasKey(e => e.ColumnId)
                   .HasName("PK_Setting_ColumnId");
            builder.Property(e => e.ColumnId)
                   .HasColumnName("ColumnId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.TableId)
                   .HasColumnName("TableId")
                   .IsRequired();

            builder.Property(e => e.ColumnName)
                   .HasColumnName("ColumnName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(2500);

            builder.Property(e => e.ColumnSortNo)
                   .HasColumnName("ColumnSortNo")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.DataType)
                   .HasColumnName("DataType")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.MaxLength)
                   .HasColumnName("MaxLength");

            builder.Property(e => e.Precision)
                   .HasColumnName("Precision");

            builder.Property(e => e.Scale)
                   .HasColumnName("Scale");

            builder.Property(e => e.CollationName)
                   .HasColumnName("CollationName")
                   .HasMaxLength(128);

            builder.Property(e => e.DefaultValue)
                   .HasColumnName("DefaultValue")
                   .HasMaxLength(2500);

            builder.Property(e => e.KeySortNo)
                   .HasColumnName("KeySortNo")
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.IsNullable)
                   .HasDefaultValue(1)
                   .IsRequired();

            builder.Property(e => e.IsOptionField)
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(e => e.FieldType)
                   .HasColumnName("FieldType")
                   .HasMaxLength(50);

            builder.Property(e => e.IsLookupField)
                    .HasDefaultValue(0)
                    .IsRequired();

            // Связь с Table
            builder.HasOne(c => c.Table)
                    .WithMany(t => t.Columns)
                    .HasForeignKey(c => c.TableId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Column_Setting_Table"); ;


            /*
            builder.HasMany(e => e.ColumnDefineDestColumn)
                   .WithOne(d => d.DestColumn)
                   .HasForeignKey(d => d.DestColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.ColumnDefineSourceColumn)
                   .WithOne(d => d.SourceColumn)
                   .HasForeignKey(d => d.SourceColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.ForeingColumnDefine)
                   .WithOne(f => f.Column)
                   .HasForeignKey(f => f.ColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.ForeingKeyDefine)
                   .WithOne(f => f.ForeingColumn)
                   .HasForeignKey(f => f.ForeingColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.IndexDefine)
                   .WithOne(i => i.Column)
                   .HasForeignKey(i => i.ColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.ReferenceColumnDefineRefColumn)
                   .WithOne(r => r.RefColumn)
                   .HasForeignKey(r => r.RefColumnId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.ReferenceColumnDefineSourceColumn)
                   .WithOne(r => r.SourceColumn)
                   .HasForeignKey(r => r.SourceColumnId)
                   .OnDelete(DeleteBehavior.NoAction);
            */

            // Apply BaseEntity configuration
            builder.ConfigureBaseEntity();
        }
    }
}
