using DecisionFlow.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Services
{
    public class ApproveDecisionService
    {
        private readonly IDecisionRepository _repository;

        public ApproveDecisionService(IDecisionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(Guid decisionId, string user)
        {
            var decision = await _repository.GetByIdAsync(decisionId);
            decision.Approve(user);
            await _repository.SaveChangesAsync();
        }
    }
}
