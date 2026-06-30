using DecisionFlow.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Services
{
    public class RejectDecisionService
    {
        private readonly IDecisionRepository _repository;

        public RejectDecisionService(IDecisionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(Guid decisionId, string user)
        {
            var decision = await _repository.GetByIdAsync(decisionId);
            decision.Reject(user);
            await _repository.SaveChangesAsync();
        }
    }
}
