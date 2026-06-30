using DecisionFlow.Application.Interfaces;
using DecisionFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionFlow.Application.Services
{
    public class RejectDecisionService
    {
        private readonly IDecisionRepository<Decision> _repository;

        public RejectDecisionService(IDecisionRepository<Decision> repository)
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
