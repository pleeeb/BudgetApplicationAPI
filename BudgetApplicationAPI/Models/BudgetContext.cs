using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetApplicationAPI.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BudgetApplicationAPI.Models
{
    public interface IBudgetContext
    {
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }

    public class BudgetContext : DbContext, IBudgetContext
    {
        public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) 
        {
            
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Budget> Budget { get; set; } = default!;
        public virtual DbSet<LinkedBudget> LinkedBudget { get; set; } = default!;
        public virtual DbSet<Transaction> Transaction { get; set; } = default!;
        public virtual DbSet<Authentication> Authentication { get; set; } = default!;
        public virtual DbSet<ApplicationSetting> ApplicationSetting { get; set; } = default!;

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
