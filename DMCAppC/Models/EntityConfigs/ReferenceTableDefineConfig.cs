using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMCApp.Models.EntityConfigs
{
    internal class ReferenceTableDefineConfig : IEntityTypeConfiguration<ReferenceTableDefine>
    {
        public void Configure(EntityTypeBuilder<ReferenceTableDefine> builder)
        {
            builder.ToTable("ReferenceTableDefine", "Setting");
            builder.HasKey(e => e.ReferenceTableDefineId)
                   .HasName("PK_Setting_ReferenceTableDefine_ReferenceTableDefineId");

            builder.Property(e => e.ReferenceTableDefineId)
                   .HasColumnName("ReferenceTableDefineId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.VersionId)
                   .HasColumnName("VersionId")
                   .IsRequired();

            builder.Property(e => e.ParentReferenceTableDefineId)
                     .HasColumnName("ParentReferenceTableDefineId");

            builder.Property(e => e.RefTableName)
                     .HasColumnName("RefTableName")
                     .HasMaxLength(128)
                     .IsRequired();

            builder.Property(e => e.RefTableId)
                     .HasColumnName("RefTableId");

            builder.Property(e => e.RefTableAliasName)
                        .HasColumnName("RefTableAliasName")
                        .HasMaxLength(128)
                        .IsRequired();

            builder.Property(e => e.CustomeTableSelect)
                        .HasColumnName("CustomeTableSelect");

            builder.Property(e => e.RefLevelNo)
                        .HasColumnName("RefLevelNo")
                        .HasMaxLength(3);

            builder.Property(e => e.RefType)
                        .HasColumnName("RefType")
                        .HasMaxLength(128);

            builder.Property(e => e.JoinType)
                        .HasColumnName("JoinType")
                        .HasMaxLength(128);

            builder.Property(e => e.Description)
                   .HasColumnName("Description")
                   .HasMaxLength(2500);


        }
    }
}
