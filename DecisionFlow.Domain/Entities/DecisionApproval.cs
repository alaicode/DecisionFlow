using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Domain.Entities
{
    public class DecisionApproval
    {
        public Guid Id { get; set; }
        public Guid DecisionId { get; set; }
        public string ApprovedBy { get; private set; }
        public DateTime ApprovedAt { get; private set; }
        public bool IsApproved { get; private set; }
        public Decision Decision { get; set; }

        public DecisionApproval(string approvedBy, bool isApproved = true)
        {
            ApprovedBy = approvedBy;
            ApprovedAt = DateTime.UtcNow;
            IsApproved = isApproved;
        }

    }
}
