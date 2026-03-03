using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class ReferenceColumnDefineConfig : IEntityTypeConfiguration<Entities.ReferenceColumnDefine>
    {
        public void Configure(EntityTypeBuilder<Entities.ReferenceColumnDefine> builder)
        {
            builder.ToTable("ReferenceColumnDefine", "Setting");
            builder.HasKey(e => e.ReferenceColumnDefineId).HasName("PK_Setting_ReferenceColumnDefine_ReferenceColumnDefineId");

            builder.Property(e => e.ReferenceColumnDefineId).ValueGeneratedOnAdd();
            builder.Property(e => e.SourceColumnName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DestColumnName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DataType).HasMaxLength(128).IsRequired();
            builder.Property(e => e.IsNullable).HasColumnType("tinyint").HasDefaultValue(1).IsRequired();
            builder.Property(e => e.IsOptionField).HasColumnType("tinyint").HasDefaultValue(0).IsRequired();
            builder.Property(e => e.FieldType).HasMaxLength(50);
            builder.Property(e => e.CollationName).HasMaxLength(255);
            builder.Property(e => e.SourceKeyColumn).HasMaxLength(128);
            builder.Property(e => e.DestKeyColumn).HasMaxLength(128);
            builder.Property(e => e.RefTableName).HasMaxLength(128);
            builder.Property(e => e.RefColumnName).HasMaxLength(128);

            builder.ConfigureBaseEntity();
        }
    }
}
