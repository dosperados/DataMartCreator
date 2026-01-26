using DMCApp.Models.EntityConfigs;
using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DMCApp.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connStr = App.CurrentConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    throw new InvalidOperationException("Connection string is not set.");
                    //optionsBuilder.UseSqlServer("Server=localhost;Database=YourDatabaseName;Trusted_Connection=True;"); // здесь нужно использовать вабранную строку подключения из appsettings.json
                }
                optionsBuilder.UseSqlServer(connStr);
            }
        }

        public virtual DbSet<Db> Dbs { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<Column> Columns { get; set; }

        /*
        public virtual DbSet<TableDefine> TableDefines { get; set; }
        public virtual DbSet<ReferenceTableDefine> ReferenceTableDefines { get; set; }
        public virtual DbSet<ReferenceColumnDefine> ReferenceColumnDefines { get; set; }
        public virtual DbSet<ColumnDefine> ColumnDefines { get; set; }
        
        public virtual DbSet<ForeingColumnDefine> ForeingColumnDefines { get; set; }
        public virtual DbSet<ForeingKeyDefine> ForeingKeyDefines { get; set; }
        public virtual DbSet<IndexDefine> IndexDefines { get; set; }
        public virtual DbSet<ColumnNameChange> ColumnNameChanges { get; set; }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); // Автоматически применит все конфигурации из папки EntityConfigs

            /* можно в ручную применять каждую конфигурацию так:
                modelBuilder.ApplyConfiguration(new DbConfiguration());
                modelBuilder.ApplyConfiguration(new TableConfiguration());
             */
            modelBuilder.ApplySoftDeleteFilters(); // Применяем глобальные фильтры мягкого удаления ко всем BaseEntity
        }
        // Переопределение SaveChanges/SaveChangesAsync для автоматического обновления ModifiedAt/ModifiedBy
        public override int SaveChanges()
        {
            LastModifieTracker();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            LastModifieTracker();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void LastModifieTracker()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            string currentUser = Environment.UserDomainName + "\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            foreach (var entityEntry in entities)
            {
                var baseEntity = (BaseEntity)entityEntry.Entity;
                baseEntity.ModifiedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                    // Устанавливаем ModifiedBy только если это новая запись и не установлено явно или отличается от текущего пользователя
                    if (string.IsNullOrEmpty(baseEntity.ModifiedBy) || baseEntity.ModifiedBy == "SYSTEM")
                        baseEntity.ModifiedBy = currentUser;

                else if (entityEntry.State == EntityState.Modified && baseEntity.ModifiedBy != currentUser)
                    baseEntity.ModifiedBy = currentUser; // Обновляем пользователя при изменении
            }
        }
    }
}
