using DecisionFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Infrastructure.Persistence
{
    public class DecisionFlowDbContext : DbContext
    {
        public DecisionFlowDbContext(DbContextOptions<DecisionFlowDbContext> options) : base(options)
        {
        }

        public DbSet<Decision> Decisions => Set<Decision>();
        public DbSet<DecisionApproval> DecisionApprovals => Set<DecisionApproval>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DecisionFlowDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
