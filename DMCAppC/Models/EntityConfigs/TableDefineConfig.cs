using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class TableDefineConfig : IEntityTypeConfiguration<TableDefine>
    {
        public void Configure(EntityTypeBuilder<TableDefine> builder)
        {
            builder.ToTable("TableDefine", "Setting");

            builder.HasKey(e => e.TableDefineId)
                   .HasName("PK_Setting_TableDefine_TableDefineId");
            builder.Property(e => e.TableDefineId)
                   .HasColumnName("TableDefineId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.VersionId)
                   .HasColumnName("VersionId")
                   .IsRequired();

            builder.Property(e => e.SourceMainDBId)
                   .HasColumnName("SourceMainDBId")
                   .IsRequired();

            builder.Property(e => e.SourceMainDBName)
                   .HasColumnName("SourceMainDBName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.SourceMainSchemaName)
                   .HasColumnName("SourceMainSchemaName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.SourceMainTableId)
                   .HasColumnName("SourceMainTableId")
                   .IsRequired();

            builder.Property(e => e.SourceMainTableName)
                   .HasColumnName("SourceMainTableName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.DestMainDBId)
                   .HasColumnName("DestMainDBId")
                   .IsRequired();

            builder.Property(e => e.DestMainDBName)
                   .HasColumnName("DestMainDBName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.DestMainSchemaName)
                   .HasColumnName("DestMainSchemaName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.DestMainTableId)
                   .HasColumnName("DestMainTableId")
                   .IsRequired();

            builder.Property(e => e.DestMainTableName)
                   .HasColumnName("DestMainTableName")
                   .HasMaxLength(128)
                   .IsRequired();

            // FK Constraints
            builder.HasOne(e => e.SourceMainDB)
                   .WithMany()
                   .HasForeignKey(e => e.SourceMainDBId)
                   .HasConstraintName("FK_Setting_TableDefine_SourceMainDBId");

            builder.HasOne(e => e.DestMainDB)
                     .WithMany()
                     .HasForeignKey(e => e.DestMainDBId)
                     .HasConstraintName("FK_Setting_TableDefine_DestMainDBId");

            builder.HasOne(e => e.SourceMainTable)
                     .WithMany()
                     .HasForeignKey(e => e.SourceMainTableId)
                     .HasConstraintName("FK_Setting_TableDefine_SourceMainTableId");
            builder.HasOne(e => e.DestMainTable)
                     .WithMany()
                     .HasForeignKey(e => e.DestMainTableId)
                     .HasConstraintName("FK_Setting_TableDefine_DestMainTableId");

            // Важно! Применяем конфигурацию BaseEntity
            builder.ConfigureBaseEntity();
        }
    }
}
