using CasbinRBAC.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CasbinRBAC.Persistance
{
    public class CasbinDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
    {
        #region Properties
        public virtual DbSet<CasbinRule<TKey>> CasbinRule { get; set; }
        #endregion

        #region Fields
        private readonly IEntityTypeConfiguration<CasbinRule<TKey>> _casbinModelConfig;
        private readonly string _defaultSchemaName;
        #endregion

        #region Ctors
        public CasbinDbContext()
        {
        }

        public CasbinDbContext(DbContextOptions<CasbinDbContext<TKey>> options, string defaultSchemaName = null, string defaultTableName = "casbin_rule") : base(options)
        {
            _casbinModelConfig = new DefaultCasbinRuleEntityTypeConfiguration<TKey>(defaultTableName);
            _defaultSchemaName = defaultSchemaName;
        }

        public CasbinDbContext(DbContextOptions<CasbinDbContext<TKey>> options, IEntityTypeConfiguration<CasbinRule<TKey>> casbinModelConfig, string defaultSchemaName = null) : base(options)
        {
            _casbinModelConfig = casbinModelConfig;
            _defaultSchemaName = defaultSchemaName;
        }

        protected CasbinDbContext(DbContextOptions options, string defaultSchemaName = null, string defaultTableName = "casbin_rule") : base(options)
        {
            _casbinModelConfig = new DefaultCasbinRuleEntityTypeConfiguration<TKey>(defaultTableName);
            _defaultSchemaName = defaultSchemaName;
        }

        protected CasbinDbContext(DbContextOptions options, IEntityTypeConfiguration<CasbinRule<TKey>> casbinModelConfig, string defaultSchemaName = null) : base(options)
        {
            _casbinModelConfig = casbinModelConfig;
            _defaultSchemaName = defaultSchemaName;
        }
        #endregion

        #region Functions
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (string.IsNullOrWhiteSpace(_defaultSchemaName) is false)
            {
                modelBuilder.HasDefaultSchema(_defaultSchemaName);
            }

            if (_casbinModelConfig is not null)
            {
                modelBuilder.ApplyConfiguration(_casbinModelConfig);
            }

        }
        #endregion
    }
}
