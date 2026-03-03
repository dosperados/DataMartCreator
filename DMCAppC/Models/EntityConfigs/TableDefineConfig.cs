using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class TableDefineConfig : IEntityTypeConfiguration<Entities.TableDefine>
    {
        public void Configure(EntityTypeBuilder<Entities.TableDefine> builder)
        {
            builder.ToTable("TableDefine", "Setting");
            builder.HasKey(e => e.TableDefineId).HasName("PK_Setting_TableDefine_TableDefineId");

            builder.Property(e => e.TableDefineId).ValueGeneratedOnAdd();
            builder.Property(e => e.SourceMainDBName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.SourceMainSchemaName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.SourceMainTableName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DestMainDBName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DestMainSchemaName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DestMainTableName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(2500);

            builder.ConfigureBaseEntity();
        }
    }
}
