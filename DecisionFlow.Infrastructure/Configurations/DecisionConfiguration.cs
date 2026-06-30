using DecisionFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Infrastructure.Configurations
{
    public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Decision> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Description).IsRequired().HasMaxLength(1000);
            builder.Property(d => d.Status).IsRequired();
            builder.HasMany(d => d.Approvals).WithOne(a => a.Decision).HasForeignKey(a => a.DecisionId);
        }
    }
}
