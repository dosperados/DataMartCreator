using DMCApp.Models.EntityConfigs;
using DMCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DMCApp.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string? _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("Connection string is not set.");
                }
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public virtual DbSet<Db> Dbs { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<Column> Columns { get; set; }
        public virtual DbSet<TableDefine> TableDefines { get; set; }
        public virtual DbSet<ReferenceTableDefine> ReferenceTableDefines { get; set; }
        public virtual DbSet<ReferenceColumnDefine> ReferenceColumnDefines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

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
                {
                    if (string.IsNullOrEmpty(baseEntity.ModifiedBy) || baseEntity.ModifiedBy == "SYSTEM")
                        baseEntity.ModifiedBy = currentUser;
                }
                else if (entityEntry.State == EntityState.Modified && baseEntity.ModifiedBy != currentUser)
                {
                    baseEntity.ModifiedBy = currentUser;
                }
            }
        }
    }
}
