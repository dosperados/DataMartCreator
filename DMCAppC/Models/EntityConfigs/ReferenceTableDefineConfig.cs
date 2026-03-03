using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class ReferenceTableDefineConfig : IEntityTypeConfiguration<Entities.ReferenceTableDefine>
    {
        public void Configure(EntityTypeBuilder<Entities.ReferenceTableDefine> builder)
        {
            builder.ToTable("ReferenceTableDefine", "Setting");
            builder.HasKey(e => e.ReferenceTableDefineId).HasName("PK_Setting_ReferenceTableDefine_ReferenceTableDefineId");

            builder.Property(e => e.ReferenceTableDefineId).ValueGeneratedOnAdd();
            builder.Property(e => e.TableDefineId).IsRequired();
            builder.Property(e => e.RefTableName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.RefTableAliasName).HasMaxLength(128);
            builder.Property(e => e.RefType).HasMaxLength(50);
            builder.Property(e => e.JoinType).HasMaxLength(50);
            builder.Property(e => e.Description).HasMaxLength(2500);

            builder.ConfigureBaseEntity();
        }
    }
}
