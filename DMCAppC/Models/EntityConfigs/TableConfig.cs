using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class TableConfig : IEntityTypeConfiguration<Entities.Table>
    {
        public void Configure(EntityTypeBuilder<Entities.Table> builder)
        {
            builder.ToTable("Table", "Setting");

            builder.HasKey(e => e.TableId)
                   .HasName("PK_Setting_Table_TableId");

            builder.Property(e => e.TableId)
                   .HasColumnName("TableId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.DBId)
                   .HasColumnName("DBId")
                   .IsRequired();

            builder.Property(e => e.VersionId)
                   .HasColumnName("VersionId")
                   .IsRequired();

            builder.Property(e => e.TableName)
                   .HasColumnName("TableName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.SchemaName)
                   .HasColumnName("SchemaName")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(e => e.Type)
                   .HasMaxLength(50);

            builder.Property(e => e.MainTableName)
                   .HasMaxLength(128);

            builder.Property(e => e.MainColumnName)
                   .HasMaxLength(128);

            builder.Property(e => e.Description)
                   .HasMaxLength(2500);

            builder.HasIndex(e => new { e.DBId, e.SchemaName, e.TableName })
                   .IsUnique()
                   .HasDatabaseName("UQ_Setting_Table_DBId_SchemaName_TableName_UniqueConstraint");

            builder.HasOne(d => d.Db)
                   .WithMany(p => p.Tables)
                   .HasForeignKey(d => d.DBId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Setting_Table_Setting_DB");

            builder.ConfigureBaseEntity();
        }
    }
}
