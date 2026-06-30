using DecisionFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Infrastructure.Configurations
{
    public class DecisionApprovalConfiguration : IEntityTypeConfiguration<DecisionApproval>
    {
        public void Configure(EntityTypeBuilder<DecisionApproval> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ApprovedBy).IsRequired().HasMaxLength(200);
            builder.Property(e => e.ApprovedAt).IsRequired();
            builder.Property(e => e.IsApproved).IsRequired();
            builder.HasOne(d => d.Decision).WithMany(d => d.Approvals).HasForeignKey(d => d.DecisionId);
        }
    }
}
