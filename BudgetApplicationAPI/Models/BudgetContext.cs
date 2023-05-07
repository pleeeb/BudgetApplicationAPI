using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetApplicationAPI.Models;

namespace BudgetApplicationAPI.Models
{
    public class BudgetContext : DbContext
    {
        public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) 
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budget { get; set; } = default!;
        public DbSet<LinkedBudget> LinkedBudget { get; set; } = default!;
        public DbSet<Transaction> Transaction { get; set; } = default!;
        public DbSet<Authentication> Authentication { get; set; } = default!;
        public DbSet<ApplicationSetting> ApplicationSetting { get; set; } = default!;
    }
}
