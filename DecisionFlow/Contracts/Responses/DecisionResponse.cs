using DecisionFlow.Domain.Entities;

namespace DecisionFlow.Contracts.Responses
{
    public record DecisionResponse(Guid Id, string Title, string Description, DecisionStatus Status, DateTime CreatedAt, List<DecisionApprovalResponse> Approvals);
}
