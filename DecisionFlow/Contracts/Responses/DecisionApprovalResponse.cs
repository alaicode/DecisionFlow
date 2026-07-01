using System;

namespace DecisionFlow.Contracts.Responses
{
    public record DecisionApprovalResponse(Guid Id, Guid DecisionId, string ApprovedBy, DateTime ApprovedAt, bool IsApproved);
}
