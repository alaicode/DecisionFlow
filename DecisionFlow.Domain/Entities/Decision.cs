using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Domain.Entities
{
    public class Decision
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DecisionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DecisionApproval> Approvals { get; set; }

        public Decision(string title, string description)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Status = DecisionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Approvals = new List<DecisionApproval>();
        }

        public void Approve(string user)
        {
            if (Status.Equals(DecisionStatus.Rejected))
            {
                throw new InvalidOperationException("Cannot approve a rejected decision.");
            }

            if(Status.Equals(DecisionStatus.Approved))
            {
                throw new InvalidOperationException("Cannot approve an already approved decision.");
            }
            var approval = new DecisionApproval(user, true);
            Approvals.Add(approval);
            Status = DecisionStatus.Approved;
        }

        public void Reject(string rejectedBy)
        {
            if (Status.Equals(DecisionStatus.Approved))
            {
                throw new InvalidOperationException("Cannot reject an already approved decision.");
            }

            if (Status.Equals(DecisionStatus.Rejected))
            {
                throw new InvalidOperationException("Cannot reject an already rejected decision.");
            }

            var approval = new DecisionApproval(rejectedBy, false);
            Approvals.Add(approval);
            Status = DecisionStatus.Rejected;
        }
    }
}
